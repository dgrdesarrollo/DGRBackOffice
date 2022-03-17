namespace SATI.BackOffice.Infraestructura.Helpers
{
    using SATI.BackOffice.Infraestructura.Entidades.Comunes;
    using SATI.BackOffice.Infraestructura.Intefaces;
    using System;

    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPostPaginationUri(QueryFilters filter, string actionUrl)
        {
            string baseUrl = $"{_baseUri}{actionUrl}";
            return new Uri(baseUrl);
        }


    }
}
