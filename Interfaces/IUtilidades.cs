using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsLiderEntrega.Interfaces
{
    public interface IUtilidades
    {
        string Desencripta(string ClaveCifrado, string Cadena);
        int CalcularComision(int valor);
    }
}
