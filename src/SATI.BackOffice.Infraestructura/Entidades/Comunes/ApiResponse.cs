using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Entidades.Comunes
{
    public class ApiResponse<T>
    {

        public ApiResponse(T data)
        {
            Data = data;

        }

        public T Data { get; set; }
        public Metadata Meta { get; set; }
    }
}
