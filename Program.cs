using System;
using System.Windows.Forms;
using WindowsLiderEntrega.Interfaces;
using WindowsLiderEntrega.Commons;


namespace WindowsLiderEntrega
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IServicioExterno services = new ServicioExterno();
            IUtilidades utilidades = new Utilidades();
            Application.Run(new CalcularForm(services, utilidades));
        }
    }
}
