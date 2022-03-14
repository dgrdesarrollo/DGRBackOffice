using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace SATI.BackOffice.Infraestructura.Entidades.Comunes
{
    public class RespuestaGenerica<T> 
    {
        public bool Ok { set; get; }
        public int ErrorCode { set; get; }
        public string Mensaje { set; get; }
        public string MensajeOriginal { set; get; }

        public int NroEntidadesObtenidasOModificadas { set; get; }

        private List<T> listaItems;
        public List<T> ListaItems
        {
            get { return listaItems; }
            set { listaItems = value; }
        }

        private T dataItem;
        public T DataItem
        {
            get { return dataItem; }
            set { dataItem = value; }
        }

        public int TotalRegs { get; set; }
        public int CantidadReg { get; set; }
        public int Pagina { get; set; }
    }
}
