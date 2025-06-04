namespace ChatIADesktop.Modelos
{
    public class RespostaOllama
    {
        public string Model { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public bool Done { get; set; }
    }
}