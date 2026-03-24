using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class AccionLugar
    {
        public int IdAccion { get; set; }

        public int IdUsuario { get; set; }

        public int IdLugar { get; set; }

        public string TipoAccion { get; set; } = null!;

        public DateTime? FechaAccion { get; set; }

        [JsonIgnore]
        public Lugar? Lugar { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
