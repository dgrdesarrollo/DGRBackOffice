using SATI.BackOffice.Infraestructura.Entidades.Comunes;
using System;

namespace SATI.BackOffice.Infraestructura.Intefaces
{
    public interface IUriService
    {
        Uri GetPostPaginationUri(QueryFilters filter, string actionUrl);
    }
}
