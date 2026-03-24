using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace Models
{
    public class Lugar
    {
        public int IdLugar { get; set; }

        public string? GooglePlaceId { get; set; }

        public int? IdMunicipio { get; set; }

        public string NombreLugar { get; set; } = null!;

        public string? Direccion { get; set; }

        public Geometry? Coordenadas { get; set; }

        public string? FotoUrl { get; set; }

        public DateTime? FechaRegistro { get; set; }

        [JsonIgnore]
        public Municipio? Municipio { get; set; }
    }
}
