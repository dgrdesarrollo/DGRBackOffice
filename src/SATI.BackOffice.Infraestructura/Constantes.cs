using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SATI.BackOffice.Infraestructura
{
    public static class Constantes
    {
        public static class StoredProcedures
        {
            public const string CAROUSEL_GET_COUNT = "spBackOffice_CarouselCantidadRegistros";
            public const string CAROUSEL_GET_ALL = "spBackOffice_CarouselGetAll";
            public const string CAROUSEL_GET_BY_ID = "spBackOffice_CarouselGetById";
            public const string CAROUSEL_INSERT = "spBackOffice_CarouselInsert";
            public const string CAROUSEL_UPDATE = "spBackOffice_CarouselUpdate";
            public const string CAROUSEL_DELETE = "spBackOffice_CarouselDelete";
            //Tipo Normativa
            public const string TIPO_NORMATIVA_GET_ALL = "spBackOffice_dl_tiposGetAll ";
            //Normativa Legal
            public const string NORMA_LEGAL_GET_COUNT = "spBackOffice_dl_documentosCantidadRegistros";
            public const string NORMA_LEGAL_GET_ALL = "spBackOffice_dl_documentosGetAll";
            public const string NORMA_LEGAL_GET_BY_ID = "spBackOffice_dl_documentosGetById";
            public const string NORMA_LEGAL_INSERT = "spBackOffice_dl_documentosInsert";
            public const string NORMA_LEGAL_UPDATE = "spBackOffice_dl_documentosUpdate";
            public const string NORMA_LEGAL_DELETE = "spBackOffice_dl_documentosDelete";
        }

        public static class MensajesError
        {
            public const string CAROUSEL_REGISTROS_NO_ENCONTRADOS = "No se encontraron registros con información del Carousel. Verifique.";
            public const string CAROUSEL_REGISTRO_NO_ENCONTRADO = "No se encontró el registro del Carousel. Verifique.";

            public const string TIPO_NORMATIVA_REGISTROS_NO_ENCONTRADOS = "No se encontraron los Tipos de Normativa. Verifique.";

            public const string NORMA_LEGAL_REGISTROS_NO_ENCONTRADOS = "No se encontraron registros de Normas Legales. Verifique.";
            public const string NORMA_LEGAL_REGISTRO_NO_ENCONTRADO = "No se encontró el registro de Norma Legal. Verifique.";


        }
    }
}
