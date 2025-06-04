using System;

namespace ChatIADesktop.Modelos
{
    public class MensagemChat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Conteudo { get; set; } = string.Empty;
        public TipoMensagem Tipo { get; set; }
        public DateTime DataHora { get; set; } = DateTime.Now;
        public string? NomeArquivo { get; set; }
        
        public bool EhUsuario => Tipo == TipoMensagem.Usuario;
        public bool EhIA => Tipo == TipoMensagem.IA;
        public bool EhSistema => Tipo == TipoMensagem.Sistema;
        public bool EhArquivo => Tipo == TipoMensagem.Arquivo;
    }
}