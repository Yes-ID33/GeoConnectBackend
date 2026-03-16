using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Municipio
    {
        public int IdMunicipio { get; set; }

        public string NombreMunicipio { get; set; } = null!;

        public string? Departamento { get; set; }
    }
}
