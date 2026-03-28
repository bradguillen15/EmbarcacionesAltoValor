using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    //Supabase REST convierte todo a minusculas, el "JsonPropertyName" le dice literalemente lo que el codigo tiene que mandar
    public class Abono
    {
        [JsonPropertyName("CompraId")]
        public long CompraId { get; set; }

        [JsonPropertyName("Monto")]
        public decimal Monto { get; set; }

        [JsonPropertyName("Tipo")]
        public string Tipo { get; set; } = "mensualidad";

        [JsonPropertyName("FechaAbono")]
        public DateTime FechaAbono { get; set; }
    }
}
