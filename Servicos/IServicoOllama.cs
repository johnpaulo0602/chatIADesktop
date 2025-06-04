using System.Threading.Tasks;
using ChatIADesktop.Modelos;

namespace ChatIADesktop.Servicos
{
    public interface IServicoOllama
    {
        /// <summary>
        /// Processa uma mensagem do usuário e retorna resposta da IA
        /// </summary>
        /// <param name="mensagem">Texto da mensagem do usuário</param>
        /// <returns>Resposta processada da IA</returns>
        Task<string> ProcessarMensagemAsync(string mensagem);
        
        /// <summary>
        /// Obtém o modelo atual em uso pelo Ollama
        /// </summary>
        /// <returns>Nome do modelo</returns>
        Task<string> ObterModeloAtualAsync();
        
        /// <summary>
        /// Lista os modelos disponíveis no Ollama
        /// </summary>
        /// <returns>Array com nomes dos modelos</returns>
        Task<string[]> ListarModelosDisponiveisAsync();

        /// <summary>
        /// Define o modelo atual a ser usado pelo Ollama
        /// </summary>
        /// <param name="modelo">Nome do modelo a ser utilizado</param>
        /// <returns>Task completada quando a operação for concluída</returns>
        Task DefinirModeloAtualAsync(string modelo);
    }
}