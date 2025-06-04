using System;
using System.Text.Json;

namespace ChatIADesktop.Utilitarios
{
    public static class ExtensoesTipo
    {
        /// <summary>
        /// Converte um objeto para JSON
        /// </summary>
        public static string ParaJson(this object objeto)
        {
            if (objeto == null) return string.Empty;
            return JsonSerializer.Serialize(objeto, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }

        /// <summary>
        /// Converte uma string JSON para o tipo especificado
        /// </summary>
        public static T DeJson<T>(this string json)
        {
            if (string.IsNullOrEmpty(json)) return default;
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Trunca uma string para o tamanho especificado
        /// </summary>
        public static string Truncar(this string texto, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return texto.Length <= tamanhoMaximo ? texto : texto.Substring(0, tamanhoMaximo) + "...";
        }
    }
}