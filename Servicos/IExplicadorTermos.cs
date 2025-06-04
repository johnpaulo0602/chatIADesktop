using System.Threading.Tasks;
using ChatIADesktop.Modelos;

namespace ChatIADesktop.Servicos
{
    public interface IExplicadorTermos
    {
        /// <summary>
        /// Explica um termo técnico de forma simplificada
        /// </summary>
        /// <param name="termo">Termo a ser explicado</param>
        /// <returns>Resultado contendo a explicação do termo</returns>
        Task<ResultadoAnalise> ExplicarTermoAsync(string termo);
    }
}