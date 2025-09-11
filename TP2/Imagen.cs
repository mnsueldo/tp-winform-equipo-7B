using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP2
{
    public class Imagen
    {
        public int Id { get; set; }
        public int IdArticulo { get; set; }
        public string ImagenUrl { get; set; }

        public override string ToString() => ImagenUrl;
    }
}
