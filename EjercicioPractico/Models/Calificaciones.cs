using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EjercicioPractico.Models
{
    public class Calificaciones
    {
        public int Id { get; set; }
        public string Nombres { get; set; }
        public string ApellidosPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string FechaNacimiento { get; set; }
        public string Grado { get; set; }
        public string Grupo { get; set; }
        public double Calificacion { get; set; }
        public string Clave { get; set; }
        public string ClaveAux { get; set; }
        public int Positions { get; set; }

    }
}
