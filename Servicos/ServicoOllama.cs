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
        private string _modeloAtual = "gemma3"; // Valor padrão caso não exista no config
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
                _modeloAtual = modeloConfig;
            
            var timeoutConfig = _configuracao.GetSection(ConstantesApp.SECAO_OLLAMA)["TimeoutSegundos"];
            if (!string.IsNullOrEmpty(timeoutConfig) && int.TryParse(timeoutConfig, out int timeout))
                _timeoutSegundos = timeout;
            
            _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSegundos);
        }

        public async Task<string> ProcessarMensagemAsync(string mensagem)
        {
            try
            {
                // Usar somente a API generate que confirmamos estar funcionando
                var requestData = new
                {
                    model = _modeloAtual,
                    prompt = mensagem,
                    stream = false
                };

                var response = await _httpClient.PostAsJsonAsync("/api/generate", requestData);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var document = JsonDocument.Parse(jsonResponse);
                var root = document.RootElement;

                // Extrair o campo "response" do JSON
                if (root.TryGetProperty("response", out var responseProperty))
                {
                    return responseProperty.GetString() ?? "Não foi possível obter uma resposta.";
                }

                return "Formato de resposta inesperado da API.";
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
            return await Task.FromResult(_modeloAtual);
        }
        
        public async Task DefinirModeloAtualAsync(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
            {
                throw new ArgumentException("O modelo não pode ser nulo ou vazio", nameof(modelo));
            }
            
            try
            {
                // Verificar se o modelo existe na lista de modelos disponíveis
                var modelos = await ListarModelosDisponiveisAsync();
                if (!modelos.Contains(modelo))
                {
                    _logger.LogWarning($"O modelo '{modelo}' não está na lista de modelos disponíveis");
                    // Ainda assim, vamos definir o modelo solicitado, pois pode ser que 
                    // o usuário esteja usando um modelo que ainda não foi listado
                }
                
                _modeloAtual = modelo;
                _logger.LogInformation($"Modelo alterado para '{modelo}'");
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao definir modelo '{modelo}'");
                throw;
            }
        }

        public async Task<string[]> ListarModelosDisponiveisAsync()
        {
            try
            {
                // Usar somente a API tags que confirmamos estar funcionando
                var response = await _httpClient.GetAsync("/api/tags");
                if (!response.IsSuccessStatusCode)
                {
                    return new[] { _modeloAtual };
                }
                
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
                            // Remover o ":latest" para exibição mais limpa
                            var modelName = name.GetString()!;
                            if (modelName.EndsWith(":latest"))
                            {
                                modelName = modelName.Substring(0, modelName.Length - 7);
                            }
                            modelos.Add(modelName);
                        }
                    }
                }
                
                if (modelos.Count > 0)
                    return modelos.ToArray();
                
                return new[] { _modeloAtual };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar modelos disponíveis");
                return new[] { _modeloAtual };
            }
        }
    }
}