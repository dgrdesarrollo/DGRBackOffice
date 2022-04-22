using SATI.BackOffice.WS.Infra.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BackOffice.WS.Tester
{
    class Program
    {
        private static ILoggerHelper _logger = new LoggerHelper();

        static void Main(string[] args)
        {
            try
            {
                PreparandoPantalla();
                Console.Clear();
                Console.Title = "Testeo de Servicio WS BackOffice";
                var procesarServicio = new ProcesarServicio();

                Thread thread = new Thread(new ThreadStart(procesarServicio.Inicio));

                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {
                _logger.Log(ex);
                throw;
            }
            Console.WriteLine("Presione una tecla...");
            Console.ReadKey();
        }

        private static void PreparandoPantalla()
        {
            Console.OutputEncoding = Encoding.Default;
            Console.WindowHeight = 40;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
