using Newtonsoft.Json;
using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace SATI.BackOffice.Infraestructura.Helpers
{
    public class HelperAPI
    {
        public HelperAPI()
        {
        }

       

        //public HttpClient InicializaCliente(string token)
        public HttpClient InicializaCliente()
        {
            //Bypass the Certificate
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };


            HttpClient client = new HttpClient(clientHandler);
            //{
            //    BaseAddress = new Uri(rutaBase)
            //};
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ////agregamos el token para la solicitud de los datos
            //if (!string.IsNullOrWhiteSpace(token))
            //{
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //}

            return client;
        }

        //public HttpClient InicializaCliente<T>(T entidad, string token, out StringContent contentData)
        public HttpClient InicializaCliente<T>(T entidad, out StringContent contentData)
        {
            //HttpClient client = InicializaCliente(token);
            HttpClient client = InicializaCliente();

            string dataJson = JsonConvert.SerializeObject(entidad);
            contentData = new StringContent(dataJson, Encoding.UTF8, "application/json");
            return client;
        }

        public string EvaluarQueryFilter(QueryFilters filters)
        {
            bool first = true;
            string cadena = string.Empty;
            foreach (var prop in filters.GetType().GetProperties())
            {
                //no evaluo la propiedad Todo
                if (prop.Name.Equals("Todo"))
                { continue; }
                var valor = prop.GetValue(filters, null);
                if (prop.PropertyType == typeof(int))
                {
                    if ((int)valor != 0)
                    {
                        ComponeCadena(ref first, ref cadena, prop, valor);
                        continue;
                    }
                }

                if (prop.PropertyType == typeof(string))
                {
                    if (!string.IsNullOrWhiteSpace((string)valor))
                    {
                        ComponeCadena(ref first, ref cadena, prop, valor);
                        continue;

                    }
                }

                if (prop.PropertyType == typeof(Nullable<DateTime>))
                {
                    if (((DateTime?)valor).HasValue)
                    {
                        ComponeCadena(ref first, ref cadena, prop, valor);
                        continue;

                    }
                }

                if (valor != null)
                {
                    ComponeCadena(ref first, ref cadena, prop, valor);
                    continue;
                }

            }

            return cadena;

        }

        private static void ComponeCadena(ref bool first, ref string cadena, PropertyInfo prop, object valor)
        {
            if (first) { first = false; }
            else { cadena += "&"; }
            cadena += $"{prop.Name}={valor}";
        }
    }
}
