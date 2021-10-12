using System.Threading.Tasks;
using WindowsLiderEntrega.ServicioPrueba;

namespace WindowsLiderEntrega.Interfaces
{
    public interface IServicioExterno
    {
        Transaccion[] GetData();
        Task<string> GetClaveCifradoCuentaAsync(long cuentaOrigen);
        void SaveData(Saldo[] saldos);
    }
}
