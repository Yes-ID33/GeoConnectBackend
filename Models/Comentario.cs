using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class Comentario
    {
        public int IdComentario { get; set; }

        public int IdUsuario { get; set; }

        public int IdLugar { get; set; }

        public string Comentario1 { get; set; } = null!;

        public float Calificacion { get; set; }

        public DateTime? FechaPublicacion { get; set; }

        [JsonIgnore]
        public Lugar? Lugar { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
