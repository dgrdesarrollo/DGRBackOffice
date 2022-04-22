using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.WS.Infra.Entidades
{
    public class QueryFilters
    {
        public string Id { get; set; }
        public int? IdRef { get; set; }
        //public Guid IdG { get; set; }
        public DateTime? Date { get; set; }
        public string FileName { get; set; }
        public string CUIT { get; set; }
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Sort { get; set; }
        public string SortDir { get; set; }
    }
}
