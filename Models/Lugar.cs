using System;
using System.Collections.Generic;
using System.Text;
using NetTopologySuite.Geometries;

namespace Models
{
    public class Lugar
    {
        public string GooglePlaceId { get; set; } = null!;

        public int? IdMunicipio { get; set; }

        public string NombreLugar { get; set; } = null!;

        public string? Direccion { get; set; }

        public Geometry Coordenadas { get; set; } = null!;

        public string? FotoUrl { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

        public virtual Municipio? IdMunicipioNavigation { get; set; }

        public virtual ICollection<AccionLugar> LugaresAcciones { get; set; } = new List<AccionLugar>();
    }
}
