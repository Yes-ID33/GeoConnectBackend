using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{

    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string Nombre { get; set; } = null!;

        public string Correo { get; set; } = null!;

        public string Contrasena { get; set; } = null!;

        public bool? Verificado { get; set; }

        public string? TokenVerificacion { get; set; }

        public DateTime? TokenExpira { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public virtual ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();

        public virtual ICollection<AccionLugar> LugaresAcciones { get; set; } = new List<AccionLugar>();
    }
}