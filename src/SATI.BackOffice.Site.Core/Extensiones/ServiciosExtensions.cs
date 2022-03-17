using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SATI.BackOffice.Infraestructura.Helpers;
using SATI.BackOffice.Infraestructura.Intefaces;
using SATI.BackOffice.Site.Core.Servicios.Contratos;
using SATI.BackOffice.Site.Core.Servicios.Implementaciones;

namespace SATI.BackOffice.Site.Core.Extensiones
{
    public static class ServiciosExtensions
    {
        public static IServiceCollection AddServicios(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerHelper, LoggerHelper>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ITipoNormaServicio, TipoNormaServicio>();
            services.AddScoped<INormaLegalServicio, NormaLegalServicio>();
            services.AddScoped<ICarruselServicio, CarruselServicio>();

            return services;
        }
    }
}
