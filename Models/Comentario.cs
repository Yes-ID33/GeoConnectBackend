using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Comentario
    {
        public int IdComentario { get; set; }

        public int? IdUsuario { get; set; }

        public string? GooglePlaceId { get; set; }

        public string Comentario1 { get; set; } = null!;

        public DateTime? FechaPublicacion { get; set; }

        public virtual Lugar? GooglePlace { get; set; }

        public virtual Usuario? IdUsuarioNavigation { get; set; }
    }
}
