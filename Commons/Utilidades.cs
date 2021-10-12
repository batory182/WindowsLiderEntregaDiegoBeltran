using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using WindowsLiderEntrega.Interfaces;

namespace WindowsLiderEntrega.Commons
{
    public class Utilidades : IUtilidades
    {
        public Utilidades()
        {

        }
        /// <summary>
        /// Metodo que obtiene el numero de primos dentro el valor de la transaccion.
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public int CalcularComision(int valor)
        {
            int total = 0;
            var x = Enumerable.Range(0, (int)Math.Floor(2.52 * Math.Sqrt(valor) / Math.Log(valor))).Aggregate(
                        Enumerable.Range(2, valor - 1).ToList(),
                          (result, index) =>
                          {
                            var bp = result[index]; var sqr = bp * bp;
                            result.RemoveAll(i => i >= sqr && i % bp == 0);
                            return result;
                          });
            total = x.Count();
            return (--total);
        }
        public string Desencripta(string ClaveCifrado, string Cadena)
        {
            //Este metodo no se requiere estructurar / optimizar
            byte[] Clave = Encoding.ASCII.GetBytes(ClaveCifrado);
            byte[] IV = Encoding.ASCII.GetBytes("1234567812345678");


            byte[] inputBytes = Convert.FromBase64String(Cadena);
            byte[] resultBytes = new byte[inputBytes.Length];
            string textoLimpio = String.Empty;
            RijndaelManaged cripto = new RijndaelManaged();
            using (MemoryStream ms = new MemoryStream(inputBytes))
            {
                using (CryptoStream objCryptoStream = new CryptoStream(ms, cripto.CreateDecryptor(Clave, IV), CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(objCryptoStream, true))
                    {
                        textoLimpio = sr.ReadToEnd();
                    }
                }
            }
            return textoLimpio;
        }

    }
}
