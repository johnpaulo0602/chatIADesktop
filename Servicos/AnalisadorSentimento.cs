using System.Threading.Tasks;
using ChatIADesktop.Modelos;

namespace ChatIADesktop.Servicos
{
    public class AnalisadorSentimento : IAnalisadorSentimento
    {
        private readonly IServicoOllama _servicoOllama;
        
        public AnalisadorSentimento(IServicoOllama servicoOllama)
        {
            _servicoOllama = servicoOllama;
        }
        
        public async Task<ResultadoAnalise> AnalisarSentimentoAsync(string texto)
        {
            var prompt = $"Análise de sentimento. Classifique o texto como positivo, negativo ou neutro. Responda apenas com uma palavra: positivo, negativo ou neutro. Texto: \"{texto}\"";
            
            var resposta = await _servicoOllama.ProcessarMensagemAsync(prompt);
            
            // Normaliza a resposta para garantir apenas uma das três opções
            var sentimentoNormalizado = NormalizarSentimento(resposta);
            
            return new ResultadoAnalise
            {
                TextoOriginal = texto,
                Resultado = sentimentoNormalizado,
                TipoAnalise = "Sentimento"
            };
        }
        
        private string NormalizarSentimento(string sentimento)
        {
            var sentimentoLower = sentimento.ToLowerInvariant().Trim();
            
            if (sentimentoLower.Contains("positiv"))
                return "Positivo";
                
            if (sentimentoLower.Contains("negativ"))
                return "Negativo";
                
            return "Neutro";
        }
    }
}