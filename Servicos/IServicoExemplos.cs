using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatIADesktop.Servicos
{
    /// <summary>
    /// Interface para o serviço que gera exemplos de perguntas
    /// </summary>
    public interface IServicoExemplos
    {
        /// <summary>
        /// Gera perguntas de exemplo para serem exibidas na página inicial
        /// </summary>
        /// <param name="quantidade">Quantidade de perguntas a serem geradas</param>
        /// <returns>Lista com as perguntas geradas</returns>
        Task<List<string>> GerarPerguntasExemploAsync(int quantidade = 2);
    }
}