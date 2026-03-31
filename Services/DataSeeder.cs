using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public static class DataSeeder
    {
        public static void Seed(GeoConnectContext context)
        {
            // 1. LISTA DE MUNICIPIOS
            var nombresMunicipios = new List<string>
            {
                "Abejorral", "Abriaquí", "Alejandría", "Amagá", "Amalfi", "Andes", "Angelópolis", "Angostura",
                "Anorí", "Anzá", "Apartadó", "Arboletes", "Argelia", "Armenia", "Barbosa", "Bello", "Belmira",
                "Betania", "Betulia", "Briceño", "Buriticá", "Cáceres", "Caicedo", "Caldas", "Campamento",
                "Cañasgordas", "Caracolí", "Caramanta", "Carepa", "Carolina del Príncipe", "Caucasia",
                "Chigorodó", "Cisneros", "Ciudad Bolívar", "Cocorná", "Concepción", "Concordia", "Copacabana",
                "Dabeiba", "Donmatías", "Ebéjico", "El Bagre", "El Carmen de Viboral", "El Peñol", "El Retiro",
                "El Santuario", "Entrerríos", "Envigado", "Fredonia", "Frontino", "Giraldo", "Girardota",
                "Gómez Plata", "Granada", "Guadalupe", "Guarne", "Guatapé", "Heliconia", "Hispania", "Itagüí",
                "Ituango", "Jardín", "Jericó", "La Ceja", "La Estrella", "La Pintada", "La Unión", "Liborina",
                "Maceo", "Marinilla", "Medellín", "Montebello", "Murindó", "Mutatá", "Nariño", "Nechí", "Necoclí",
                "Olaya", "Peque", "Pueblorrico", "Puerto Berrío", "Puerto Nare", "Puerto Triunfo", "Remedios",
                "Rionegro", "Sabanalarga", "Sabaneta", "Salgar", "San Andrés de Cuerquia", "San Carlos",
                "San Francisco", "San Jerónimo", "San José de la Montaña", "San Juan de Urabá", "San Luis",
                "San Pedro de los Milagros", "San Pedro de Urabá", "San Rafael", "San Roque", "San Vicente Ferrer",
                "Santa Bárbara", "Santa Fe de Antioquia", "Santa Rosa de Osos", "Santo Domingo", "Segovia",
                "Sonsón", "Sopetrán", "Támesis", "Tarazá", "Tarso", "Titiribí", "Toledo", "Turbo", "Uramita",
                "Urrao", "Valdivia", "Valparaíso", "Vegachí", "Venecia", "Vigía del Fuerte", "Yalí", "Yarumal",
                "Yolombó", "Yondó", "Zaragoza"
            };

            bool cambiosMunicipios = false;

            // VERIFICACIÓN FILA POR FILA (IF NOT EXISTS)
            foreach (var nombre in nombresMunicipios)
            {
                if (!context.Municipios.Any(m => m.NombreMunicipio == nombre))
                {
                    context.Municipios.Add(new Municipio
                    {
                        NombreMunicipio = nombre,
                        Departamento = "Antioquia"
                    });
                    cambiosMunicipios = true;
                }
            }

            // Guardamos solo si hubo inserciones nuevas
            if (cambiosMunicipios)
            {
                context.SaveChanges();
            }

            // 2. LISTA DE LUGARES TURÍSTICOS
            var listaLugares = new List<(string Nombre, string Direccion, string Municipio)>
            {
                ("Pueblito Paisa", "Calle 30A #55-64, Cerro Nutibara", "Medellín"),
                ("Museo de arte moderno", "Cra. 44 #19a-100", "Medellín"),
                ("Estación del ferrocarril", "Carrera 20 # 19-05 (A un costado del Parque Principal)", "Cisneros"),
                ("Parque Arví", "Vía a Piedras Blancas, Corregimiento de Santa Elena", "Medellín"),
                ("Jardín colonial", "En todo el municipio", "Carolina del Príncipe"),
                ("La Ruta Lechera", "Vía principal Norte (Entrada al municipio)", "San Pedro de los Milagros"),
                ("Puente de occidente", "Vía Santa Fe de Antioquia - Sucre (Sobre el río Cauca)", "Santa Fe de Antioquia"),
                ("Piedra del peñol", "Km 2 vía guatape - el peñol", "Guatapé"),
                ("Plaza botero", "Cra. 52 #52-43", "Medellín"),
                ("Jardín botánico", "Calle 73 #51D-14", "Medellín"),
                ("Parque Explora", "Cra. 52 #73-75", "Medellín"),
                ("Cueva del Esplendor", "Alto de las Flores, Vereda La Linda", "Jardín"),
                ("Basílica Menor de la Inmaculada Concepción", "Parque principal", "Jardín"),
                ("Cerro tusa", "Vía venecia-bolombolo", "Venecia"),
                ("Parque Temático Hacienda Nápoles", "Km 165 Vía Medellín - Bogotá", "Puerto Triunfo"),
                ("Salto del Tequendamita", "Vía La Ceja - Don Diego", "El Retiro"),
                ("Casa Museo Otraparte", "Cra. 43A #27A Sur-11", "Envigado"),
                ("Museo de Antioquia", "Calle 52 #52-43", "Medellín"),
                ("Santuario de la Madre Laura", "Cra. 92 #34D-21, Barrio Belencito", "Jericó")
            };

            // Traemos los municipios a memoria para relacionarlos
            var municipiosDb = context.Municipios.ToList();
            bool cambiosLugares = false;

            foreach (var item in listaLugares)
            {
                var muni = municipiosDb.FirstOrDefault(m =>
                    m.NombreMunicipio.Equals(item.Municipio, StringComparison.OrdinalIgnoreCase));

                if (muni != null)
                {
                    // VERIFICACIÓN FILA POR FILA PARA LUGARES (Comparamos por Nombre y Municipio)
                    if (!context.Lugares.Any(l => l.NombreLugar == item.Nombre && l.IdMunicipio == muni.IdMunicipio))
                    {
                        context.Lugares.Add(new Lugar
                        {
                            NombreLugar = item.Nombre,
                            Direccion = item.Direccion,
                            IdMunicipio = muni.IdMunicipio,
                            FechaRegistro = DateTime.Now
                        });
                        cambiosLugares = true;
                    }
                }
            }

            if (cambiosLugares)
            {
                context.SaveChanges();
            }
        }
    }
}