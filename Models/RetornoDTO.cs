using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Capta.WPF.Models
{
    public class RetornoDTO
    {
        public bool success { get; set; }
        public string? message { get; set; }
        public List<ClienteDTO> data { get; set; }

    }
}
