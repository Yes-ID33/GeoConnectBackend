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
                ("Basílica de la Inmaculada Concepción", "Parque principal", "Jardín"),
                ("Cerro tusa", "Vía venecia-bolombolo", "Venecia"),
                ("Parque Temático Hacienda Nápoles", "Km 165 Vía Medellín - Bogotá", "Puerto Triunfo"),
                ("Salto del Tequendamita", "Vía La Ceja - Don Diego", "El Retiro"),
                ("Casa Museo Otraparte", "Cra. 43A #27A Sur-11", "Envigado"),
                ("Museo de Antioquia", "Calle 52 #52-43", "Medellín"),
                ("Santuario de la Madre Laura", "Cl. 10 #455", "Jericó"),
                ("Cañón del Río Claro", "Km 140, Autopista Medellín-Bogotá", "San Luis"),
                ("Museo Casa de la Memoria", "Calle 51 # 36-66, Parque Bicentenario", "Medellín"),
                ("Salto del Buey", "Vereda El Higuerón (Vía La Ceja - Abejorral)", "La Ceja"),
                ("Cementerio Museo San Pedro", "Carrera 51 # 68-68", "Medellín"),
                ("Mirador del Alto de San Miguel", "Vereda La Mina", "Caldas"),
                ("Cerro El Volador", "Calle 67 # 67-27", "Medellín"),
                ("Museo El Castillo", "Carrera 39D # 27D Sur", "Envigado"),
                ("Pueblo Tutucán", "Carrera 55A # 35-229 (Parque Comfama)", "Rionegro"),
                ("Escaleras Eléctricas Comuna 13", "Carrera 110 # 34CC, Barrio Las Independencias", "Medellín"),
                ("Piedra del Tabor", "Vereda El Tabor (Sendero de ascenso)", "San Carlos"),
                ("Santuario del Señor Caído", "Carrera 15 # 6-64, Parque Principal", "Girardota"),
                ("Cascada del Amor", "Km 16 vía Cocorná - Granada", "Cocorná"),
                ("Parque Principal de Concepción", "Calle 20 # 20-02 (Parque José María Córdova)", "Concepción"),
                ("Termales de Alejandría", "Km 2 vía Alejandría - Santo Domingo", "Alejandría"),
                ("Salto del Diablo", "Vía a la vereda El Líbano", "Támesis"),
                ("Parques del Río", "Entre la Calle 33 y Calle 44 (Autopista Sur)", "Medellín"),
                ("Cerro de las Tres Cruces", "Calle 8 # 84F-25 (Acceso por Belén)", "Medellín"),
                ("Balneario Las Tangas", "Vía San Rafael - Guatapé", "San Rafael"),
                ("Museo de Artes Visuales", "Calle 51 # 50-62", "Itagüí"),
                ("Volcán de Lodo", "Vereda El Volcán, a orillas del mar", "Arboletes"),
                ("La Casa en el Aire", "Vereda La Polka, Cerro San Vicente", "Abejorral"),
                ("Páramo del Sol", "Acceso por la Vereda La Nevera", "Urrao"),
                ("Réplica del Viejo Peñol", "Km 12 vía Marinilla - El Peñol", "El Peñol"),
                ("Circo Teatro", "Carrera 20 # 19-35", "Titiribí"),
                ("Parque de las Aguas", "Km 23 Autopista Norte", "Barbosa"),
                ("Catedral Nuestra Señora de Chiquinquirá", "Calle 30 # 30-02", "Santa Rosa de Osos"),
                ("Petroglifos de Támesis", "Sendero arqueológico Vereda San Luis", "Támesis"),
                ("Museo de Arte Religioso Tiburcio Álvarez", "Carrera 7 # 8-02", "Sonsón"),
                ("Parque Ecológico Los Salados", "Km 25 Vía Las Palmas", "El Retiro"),
                ("Cueva de los Guácharos", "Vereda La Selva", "San Roque"),
                ("Charco Azul", "Vereda El Venado", "La Unión"),
                ("Planetario de Medellín", "Carrera 52 # 71-117 Carrera 51 # 52-03", "Medellín"),
                ("Palacio de la Cultura Rafael Uribe Uribe", "Carrera 51 # 52-03 (Plaza Botero)", "Medellín"),
                ("Museo MAJA (Antropología y Arte)", "Calle 7 # 4-07", "Jericó"),
                ("Parque de los Deseos", "Calle 71 # 52-30", "Medellín"),
                ("Templo de Piedra", "Vereda El Salto", "Gómez Plata"),
                ("Puente Monumental sobre el Río Magdalena", "Conexión con Puerto Olaya", "Puerto Berrío"),
                ("Parque Ecológico La Romera", "Sector La Romera (Zona alta)", "Sabaneta"),
                ("Ecoparque El Gaitero", "Vereda La Linda", "Sopetrán"),
                ("Museo Histórico de El Peñol", "Carrera 21 # 18-35", "El Peñol"),
                ("Cascada El Salto del Ángel", "Vereda Quebradona", "Jericó"),
                ("Cerro Bravo", "Kilómetro 5 vía Fredonia", "Fredonia"),
                ("Teatro Pablo Tobón Uribe", "Carrera 40 # 51-24", "Medellín")
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