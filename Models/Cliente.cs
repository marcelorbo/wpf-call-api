using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teste.Capta.WPF.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? CPF { get; set; }
        public string? Sexo { get; set; }
        public int IdTipoCliente { get; set; }
        public int IdSituacaoCliente { get; set; }
    }
}
