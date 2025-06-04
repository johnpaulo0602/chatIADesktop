using System.Threading.Tasks;
using ChatIADesktop.Modelos;

namespace ChatIADesktop.Servicos
{
    public class ExplicadorTermos : IExplicadorTermos
    {
        private readonly IServicoOllama _servicoOllama;
        
        public ExplicadorTermos(IServicoOllama servicoOllama)
        {
            _servicoOllama = servicoOllama;
        }
        
        public async Task<ResultadoAnalise> ExplicarTermoAsync(string termo)
        {
            var prompt = $"Explique o termo \"{termo}\" em linguagem simples e clara, com no máximo 100 palavras, como se estivesse explicando para alguém sem conhecimento técnico.";
            
            var explicacao = await _servicoOllama.ProcessarMensagemAsync(prompt);
            
            return new ResultadoAnalise
            {
                TextoOriginal = termo,
                Resultado = explicacao,
                TipoAnalise = "Explicação"
            };
        }
    }
}