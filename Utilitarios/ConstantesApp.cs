namespace ChatIADesktop.Utilitarios
{
   public static class ConstantesApp
   {
         // Configurações do Ollama
         public const string SECAO_OLLAMA = "Ollama";

         // Tamanhos e limites
         public const int TAMANHO_MAXIMO_RESUMO = 500;

         // Mensagens do sistema
         public const string ERRO_CONEXAO_OLLAMA = "Não foi possível conectar ao serviço Ollama.";
         public const string ERRO_TIMEOUT_OLLAMA = "A solicitação ao Ollama excedeu o tempo limite.";
         public const string ERRO_PROCESSAMENTO_ARQUIVO = "Erro ao processar o arquivo.";
   }
}
