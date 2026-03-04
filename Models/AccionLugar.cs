using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AccionLugar
    {
        public int IdAccion { get; set; }

        public int? IdUsuario { get; set; }

        public string? GooglePlaceId { get; set; }

        public string TipoAccion { get; set; } = null!;

        public DateTime? FechaAccion { get; set; }

        public virtual Lugar? GooglePlace { get; set; }

        public virtual Usuario? IdUsuarioNavigation { get; set; }
    }
}
