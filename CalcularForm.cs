//PRUEBA OLIMPIA
//La función de la aplicación actual es calcular el saldo final de las cuentas de un "banco", para esto se consume un servicio que devuelve 
//las transacciones realizas a la cuentas.

//Paso 1: Hacer funcionar la aplicación. Debido al aumento de transacciones y al  colocar al servicio con SSL la aplicación actual esta fallando.
//Paso 2: Estructurar mejor el codigo. Uso de patrones, buenas practicas, etc.
//Paso 3: Optimizar el codigo, como se menciono en el paso 1 el aumento de transacciones ha causado que el calculo de los saldos se demore demasiado.
//Paso 4: Adicionar una barra de progreso al formulario. Actualizar la barra con el progreso del proceso, evitando bloqueos del GUI.


using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using WindowsLiderEntrega.DTO;
using System.Diagnostics;
using WindowsLiderEntrega.ServicioPrueba;
using WindowsLiderEntrega.Interfaces;

namespace WindowsLiderEntrega
{
    public partial class CalcularForm : Form
    {
        private readonly IServicioExterno _service;
        private readonly IUtilidades _utilidades;
        public CalcularForm(IServicioExterno service, IUtilidades utilidades)
        {
            InitializeComponent();
            _service = service;
            _utilidades = utilidades;
        }

        private void btnCalcular_Click(object sender, EventArgs e)
        {
            progressBar.Visible = true;
            progressBar.Minimum = 1;
            progressBar.Maximum = 100;

            progressBar.Value = 10;
            progressBar.Step = 1;
            Stopwatch sw = Stopwatch.StartNew();

            var resp = _service.GetData();
            progressBar.PerformStep();
            //Variable donse se almacenan los saldos finales
            List<Saldo> saldos = new List<Saldo>();

            //Lista que recupera las llaves por cada cuenta
            var keyCuentas = resp.GroupBy(x => x.CuentaOrigen).Select(y => new ClaveCuentaDTO {
                CuentaOrigen = y.Key,
                ClaveCifrado = ObtenerClave(y.Key)

            }).ToList();

            // lista que realiza la union con la llave de cuenta,y obtiene el valor de la transacción is es debito o credito
            var precalculo = resp.Join(keyCuentas,
                re => re.CuentaOrigen,
                kc => kc.CuentaOrigen,
                (x, y) => new {
                    x.CuentaOrigen,
                    x.TipoTransaccion,
                    x.ValorTransaccion,
                    Valor = ObtenerValor(y.ClaveCifrado, x.TipoTransaccion, x.ValorTransaccion)
                }).ToList();

            progressBar.PerformStep();
          
            // agrupa por cuenta y obtiene la suma de las transacciones de cada cuenta.
            saldos = precalculo.GroupBy(x => x.CuentaOrigen).Select(y => new Saldo {
                CuentaOrigen = y.Key,
                SaldoCuenta = y.Sum(x => x.Valor)
            }).ToList();
            progressBar.PerformStep();

            progressBar.Visible = false;
            sw.Stop();

            lblTiempoTotal.Text = sw.ElapsedMilliseconds.ToString();

            var sal = JsonConvert.SerializeObject(saldos.OrderBy(x => x.CuentaOrigen));
            //Enviamos los saldos finales
            _service.SaveData(saldos.ToArray());
        }

        /// <summary>
        /// Obtiene la cla
        /// </summary>
        /// <param name="CuentaOrigen"></param>
        /// <returns></returns>
        private string ObtenerClave(long CuentaOrigen)
        {
            progressBar.PerformStep();
            return _service.GetClaveCifradoCuentaAsync(CuentaOrigen).GetAwaiter().GetResult();
        }
        /// <summary>
        /// Metodo que obtiene el valor de cada transaccion
        /// </summary>
        /// <param name="claveCifrado"></param>
        /// <param name="tipoTransaccion"></param>
        /// <param name="ValorTransaccion"></param>
        /// <returns></returns>
        private double ObtenerValor(string claveCifrado, string tipoTransaccion, double ValorTransaccion)
        {
            double result = 0;

            if (_utilidades.Desencripta(claveCifrado, tipoTransaccion) == "Debito")
                result = ValorTransaccion - (ValorTransaccion * 2);
            else
                result = ValorTransaccion - _utilidades.CalcularComision(Convert.ToInt32(ValorTransaccion));
            
            return result;
        }
    }
}
