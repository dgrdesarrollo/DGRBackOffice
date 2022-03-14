﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura.Entidades.Comunes
{
    public class QueryFilters 
    {
        public bool Todo { get { return (Id == default || Id == null) && IdRef == default && !Date.HasValue && string.IsNullOrWhiteSpace(Search) && PageSize == default && PageNumber == default; } }
        public object Id { get; set; }
        public int IdRef { get; set; }
        public Guid IdG { get; set; }
        public DateTime? Date { get; set; }
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string Sort { get; set; }
        public string SortDir { get; set; }

    }
}
