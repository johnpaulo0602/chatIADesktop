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
                var requestData = new
                {
                    model = _modeloPadrao,
                    prompt = mensagem,
                    stream = false
                };

                var response = await _httpClient.PostAsJsonAsync("/api/generate", requestData);
                response.EnsureSuccessStatusCode();

                var ollamaResponse = await response.Content.ReadFromJsonAsync<RespostaOllama>();
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
                var response = await _httpClient.GetAsync("/api/tags");
                response.EnsureSuccessStatusCode();
                
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
                }
                
                return modelos.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar modelos disponíveis");
                return new[] { _modeloPadrao };
            }
        }
    }
}