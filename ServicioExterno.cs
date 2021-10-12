using System.Configuration;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WindowsLiderEntrega.Interfaces;
using WindowsLiderEntrega.ServicioPrueba;

namespace WindowsLiderEntrega
{
    public class ServicioExterno: IServicioExterno
    {
        private readonly ServiceClient client = new ServiceClient();
        private readonly string pass;
        private readonly string user;
        public ServicioExterno()
        {
            //Deshabilita los Errores de Certificado, pero para el certificado del servicio
            ServicePointManager.ServerCertificateValidationCallback += delegate (object sender2, X509Certificate cert, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    return true; //Is valid
                }
                if (cert.GetCertHashString() == "C8FE3CA96EC4648787A43E73F1E5A1EB44AB02D4")
                {
                    return true;
                }
                return false;
            };
            user = ConfigurationManager.AppSettings["User"];
            pass = ConfigurationManager.AppSettings["Password"];
        }
        /// <summary>
        /// MEtodo que recupera del servicio las transacciones
        /// </summary>
        /// <returns></returns>
        public Transaccion[] GetData()
        {
            return client.GetData(user, pass);
        }

        /// <summary>
        /// Metodo que obtiene del servicio la llave de cifrado de cada cuenta
        /// </summary>
        /// <param name="cuentaOrigen"></param>
        /// <returns></returns>
        public Task<string> GetClaveCifradoCuentaAsync(long cuentaOrigen)
        {
            return client.GetClaveCifradoCuentaAsync(user, pass, cuentaOrigen);
        }
       /// <summary>
       /// Metodo que guarda los saldos.
       /// </summary>
       /// <param name="saldos"></param>
        public void SaveData(Saldo[] saldos)
        {
            client.SaveData(user, pass, saldos);
        }

    }
}
