using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.WS.Infra.Entidades
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

    public class Metadata
    {
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public string NextPageUrl { get; set; }
        public string PreviousPageUrl { get; set; }
    }
}
