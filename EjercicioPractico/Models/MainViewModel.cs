using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EjercicioPractico.Models
{
    public class MainViewModel
    {
        public string BestGrade { get; set; }
        public string WorstGrade { get; set; }
        public double Average { get; set; }
        public List<Calificaciones> Calificaciones { get; set; }
        public DataItem[] Grafica { get; set; }

    }
}
