using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ChatIADesktop.Modelos;
using ChatIADesktop.Utilitarios;

namespace ChatIADesktop.Servicos
{
    public class ServicoOllama : IServicoOllama
    {
        private readonly IConfiguration _configuracao;
        private readonly ILogger<ServicoOllama> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _modeloPadrao = "llama2"; // Valor padrão caso não exista no config
        private readonly int _timeoutSegundos = 30; // Valor padrão caso não exista no config

        public ServicoOllama(IConfiguration configuracao, ILogger<ServicoOllama> logger, HttpClient httpClient)
        {
            _configuracao = configuracao;
            _logger = logger;
            _httpClient = httpClient;
            
            var baseUrl = _configuracao.GetSection(ConstantesApp.SECAO_OLLAMA)["BaseUrl"];
            if (!string.IsNullOrEmpty(baseUrl))
                _httpClient.BaseAddress = new Uri(baseUrl);
            else
                _httpClient.BaseAddress = new Uri("http://localhost:11434"); // Valor padrão
            
            var modeloConfig = _configuracao.GetSection(ConstantesApp.SECAO_OLLAMA)["ModeloPadrao"];
            if (!string.IsNullOrEmpty(modeloConfig))
                _modeloPadrao = modeloConfig;
            
            var timeoutConfig = _configuracao.GetSection(ConstantesApp.SECAO_OLLAMA)["TimeoutSegundos"];
            if (!string.IsNullOrEmpty(timeoutConfig) && int.TryParse(timeoutConfig, out int timeout))
                _timeoutSegundos = timeout;
            
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSegundos);
        }

        public async Task<string> ProcessarMensagemAsync(string mensagem)
        {
            try
            {
                // Tenta usar a API de chat primeiro (Ollama v0.1.14+)
                try
                {
                    var chatRequestData = new
                    {
                        model = _modeloPadrao,
                        messages = new[]
                        {
                            new { role = "user", content = mensagem }
                        },
                        stream = false
                    };

                    var chatResponse = await _httpClient.PostAsJsonAsync("/api/chat", chatRequestData);
                    if (chatResponse.IsSuccessStatusCode)
                    {
                        var jsonResponse = await chatResponse.Content.ReadAsStringAsync();
                        var document = JsonDocument.Parse(jsonResponse);
                        var root = document.RootElement;

                        if (root.TryGetProperty("message", out var messageElement) && 
                            messageElement.TryGetProperty("content", out var contentElement))
                        {
                            return contentElement.GetString() ?? "Sem resposta";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao usar API de chat, tentando fallback para API generate");
                    // Continuar com o fallback abaixo
                }

                // Fallback para a API generate (versões mais antigas do Ollama)
                var generateRequestData = new
                {
                    model = _modeloPadrao,
                    prompt = mensagem,
                    stream = false
                };

                var generateResponse = await _httpClient.PostAsJsonAsync("/api/generate", generateRequestData);
                generateResponse.EnsureSuccessStatusCode();

                var ollamaResponse = await generateResponse.Content.ReadFromJsonAsync<RespostaOllama>();
                return ollamaResponse?.Response ?? "Não foi possível obter uma resposta do modelo.";
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro na comunicação com API Ollama");
                throw new Exception(ConstantesApp.ERRO_CONEXAO_OLLAMA, ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout na comunicação com API Ollama");
                throw new Exception(ConstantesApp.ERRO_TIMEOUT_OLLAMA, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem");
                throw;
            }
        }

        public async Task<string> ObterModeloAtualAsync()
        {
            return await Task.FromResult(_modeloPadrao);
        }

        public async Task<string[]> ListarModelosDisponiveisAsync()
        {
            try
            {
                // Tenta usar o endpoint models (versões mais recentes)
                try
                {
                    var response = await _httpClient.GetAsync("/api/models");
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var document = JsonDocument.Parse(jsonResponse);
                        var modelos = new List<string>();
                        
                        var root = document.RootElement;
                        if (root.TryGetProperty("models", out var modelArray) && modelArray.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var model in modelArray.EnumerateArray())
                            {
                                if (model.TryGetProperty("name", out var name) && !string.IsNullOrEmpty(name.GetString()))
                                {
                                    modelos.Add(name.GetString()!);
                                }
                            }
                            
                            return modelos.ToArray();
                        }
                    }
                }
                catch
                {
                    // Continuar com o fallback
                }
                
                // Fallback para endpoint tags (versões mais antigas)
                var tagsResponse = await _httpClient.GetAsync("/api/tags");
                tagsResponse.EnsureSuccessStatusCode();
                
                var tagsJsonResponse = await tagsResponse.Content.ReadAsStringAsync();
                var tagsDocument = JsonDocument.Parse(tagsJsonResponse);
                
                var tagsModelos = new List<string>();
                var tagsRoot = tagsDocument.RootElement;
                
                if (tagsRoot.TryGetProperty("models", out var tagsModelArray) && tagsModelArray.ValueKind == JsonValueKind.Array)
                {
                    foreach (var model in tagsModelArray.EnumerateArray())
                    {
                        if (model.TryGetProperty("name", out var name) && !string.IsNullOrEmpty(name.GetString()))
                        {
                            tagsModelos.Add(name.GetString()!);
                        }
                    }
                }
                
                if (tagsModelos.Count > 0)
                    return tagsModelos.ToArray();
                
                // Se nenhuma API funcionou, retorna apenas o modelo padrão
                return new[] { _modeloPadrao };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar modelos disponíveis");
                return new[] { _modeloPadrao };
            }
        }
    }
}