using System.Threading.Tasks;
using ChatIADesktop.Modelos;

namespace ChatIADesktop.Servicos
{
    public interface IAnalisadorSentimento
    {
        /// <summary>
        /// Analisa o sentimento de um texto
        /// </summary>
        /// <param name="texto">Texto a ser analisado</param>
        /// <returns>Resultado da análise de sentimento</returns>
        Task<ResultadoAnalise> AnalisarSentimentoAsync(string texto);
    }
}