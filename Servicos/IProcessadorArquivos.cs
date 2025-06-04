using System.IO;
using System.Threading.Tasks;
using ChatIADesktop.Modelos;

namespace ChatIADesktop.Servicos
{
    public interface IProcessadorArquivos
    {
        /// <summary>
        /// Processa um arquivo e extrai o texto
        /// </summary>
        /// <param name="stream">Stream do arquivo</param>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        /// <returns>Texto extraído do arquivo</returns>
        Task<string> ProcessarArquivoAsync(Stream stream, string nomeArquivo);
        
        /// <summary>
        /// Gera um resumo do texto
        /// </summary>
        /// <param name="texto">Texto a ser resumido</param>
        /// <returns>Resumo do texto</returns>
        Task<string> GerarResumoAsync(string texto);
        
        /// <summary>
        /// Verifica se o tipo de arquivo é suportado
        /// </summary>
        /// <param name="nomeArquivo">Nome do arquivo</param>
        /// <returns>True se o arquivo for suportado</returns>
        bool ArquivoSuportado(string nomeArquivo);
    }
}