using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1Beta.Dto
{
    public class Cliente
    {
        // hacer un campo de la Clase
        public  string UrlVideos = "https://xvideos.com";
        // clase sellada o piticlina
         // sealed
         // sealed
         
        public int id { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string Dni { get; set; }
        public string correo { get; set; }
        public string ColorPezon { get; set; }
    }
}
