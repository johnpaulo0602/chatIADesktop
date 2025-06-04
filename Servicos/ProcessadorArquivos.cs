using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using ChatIADesktop.Modelos;
using ChatIADesktop.Utilitarios;

namespace ChatIADesktop.Servicos
{
    public class ProcessadorArquivos : IProcessadorArquivos
    {
        private readonly IServicoOllama _servicoOllama;
        private readonly string[] _extensoesSuportadas = { ".pdf", ".txt" };
        
        public ProcessadorArquivos(IServicoOllama servicoOllama)
        {
            _servicoOllama = servicoOllama;
        }
        
        public bool ArquivoSuportado(string nomeArquivo)
        {
            if (string.IsNullOrEmpty(nomeArquivo)) return false;
            
            var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
            return _extensoesSuportadas.Contains(extensao);
        }
        
        public async Task<string> ProcessarArquivoAsync(Stream stream, string nomeArquivo)
        {
            try
            {
                var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();
                
                switch (extensao)
                {
                    case ".pdf":
                        return await ExtrairTextoPdfAsync(stream);
                    case ".txt":
                        return await ExtrairTextoTxtAsync(stream);
                    default:
                        throw new NotSupportedException($"Formato de arquivo não suportado: {extensao}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ConstantesApp.ERRO_PROCESSAMENTO_ARQUIVO, ex);
            }
        }
        
        public async Task<string> GerarResumoAsync(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            
            // Se o texto for menor que o limite, retorna ele mesmo
            if (texto.Length <= ConstantesApp.TAMANHO_MAXIMO_RESUMO)
                return texto;
                
            try
            {
                // Utiliza o próprio Ollama para gerar um resumo
                var prompt = $"Resumir o seguinte texto em no máximo {ConstantesApp.TAMANHO_MAXIMO_RESUMO} caracteres: {texto}";
                var resumo = await _servicoOllama.ProcessarMensagemAsync(prompt);
                
                // Garante que o resumo não exceda o tamanho máximo
                return resumo.Truncar(ConstantesApp.TAMANHO_MAXIMO_RESUMO);
            }
            catch
            {
                // Fallback para truncamento simples se o resumo falhar
                return texto.Truncar(ConstantesApp.TAMANHO_MAXIMO_RESUMO);
            }
        }
        
        private async Task<string> ExtrairTextoPdfAsync(Stream stream)
        {
            var texto = new StringBuilder();
            
            using (var pdfReader = new PdfReader(stream))
            using (var pdfDocument = new PdfDocument(pdfReader))
            {
                var totalPaginas = pdfDocument.GetNumberOfPages();
                
                for (int i = 1; i <= totalPaginas; i++)
                {
                    var page = pdfDocument.GetPage(i);
                    var strategy = new SimpleTextExtractionStrategy();
                    var conteudoPagina = PdfTextExtractor.GetTextFromPage(page, strategy);
                    
                    texto.AppendLine(conteudoPagina);
                }
            }
            
            return await Task.FromResult(texto.ToString());
        }
        
        private async Task<string> ExtrairTextoTxtAsync(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}