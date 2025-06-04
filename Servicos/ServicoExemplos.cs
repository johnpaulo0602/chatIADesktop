using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace ChatIADesktop.Servicos
{
    /// <summary>
    /// Serviço para gerar exemplos de perguntas usando o Ollama
    /// </summary>
    public class ServicoExemplos : IServicoExemplos
    {
        private readonly IServicoOllama _servicoOllama;
        private readonly ILogger<ServicoExemplos> _logger;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY = "PerguntasExemplo";
        private static readonly TimeSpan CACHE_DURATION = TimeSpan.FromHours(1);

        public ServicoExemplos(IServicoOllama servicoOllama, ILogger<ServicoExemplos> logger, IMemoryCache cache)
        {
            _servicoOllama = servicoOllama ?? throw new ArgumentNullException(nameof(servicoOllama));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <inheritdoc/>
        public async Task<List<string>> GerarPerguntasExemploAsync(int quantidade = 2)
        {
            // Verifica se já existe em cache
            if (_cache.TryGetValue<List<string>>(CACHE_KEY, out var perguntasCacheadas) && 
                perguntasCacheadas != null && 
                perguntasCacheadas.Count >= quantidade)
            {
                return perguntasCacheadas;
            }

            try
            {
                var prompt = $"Gere {quantidade} sugestões de perguntas que os usuários poderiam fazer para você em um chat. " +
                             "As perguntas devem ser simples, interessantes e variadas, preferencialmente que mostrem suas capacidades. " +
                             "Retorne apenas as perguntas, uma por linha, sem numeração ou formatação adicional.";

                var resposta = await _servicoOllama.ProcessarMensagemAsync(prompt);
                var linhas = resposta.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                
                var perguntas = new List<string>();
                foreach (var linha in linhas)
                {
                    // Ignora linhas que parecem ser explicações em vez de perguntas
                    if (linha.Length > 10 && linha.Length < 100 && (linha.EndsWith("?") || linha.StartsWith("Me ") || linha.StartsWith("Como ")))
                    {
                        perguntas.Add(linha.Trim());
                        if (perguntas.Count >= quantidade)
                            break;
                    }
                }

                // Se não conseguimos extrair perguntas suficientes, adicione algumas padrão
                if (perguntas.Count < quantidade)
                {
                    var perguntasPadrao = new List<string>
                    {
                        "Me conte uma piada!",
                        "Me ajude a escrever um poema.",
                        "Explique como a inteligência artificial funciona.",
                        "Quais são as aplicações do aprendizado de máquina?"
                    };

                    foreach (var perguntaPadrao in perguntasPadrao)
                    {
                        if (!perguntas.Contains(perguntaPadrao))
                        {
                            perguntas.Add(perguntaPadrao);
                            if (perguntas.Count >= quantidade)
                                break;
                        }
                    }
                }

                // Salva no cache por 1 hora
                _cache.Set(CACHE_KEY, perguntas, CACHE_DURATION);
                
                return perguntas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar perguntas de exemplo");
                
                // Em caso de erro, retorna perguntas padrão
                var perguntasPadrao = new List<string>
                {
                    "Me conte uma piada!",
                    "Me ajude a resolver um problema."
                };
                
                return perguntasPadrao;
            }
        }
    }
}