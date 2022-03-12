using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Capta.WPF.Models
{
    public class RetornoDominioDTO
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public List<DominioDTO> data { get; set; }
    }
}
