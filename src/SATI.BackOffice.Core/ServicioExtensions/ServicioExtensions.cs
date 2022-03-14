using Microsoft.Extensions.DependencyInjection;
using SATI.BackOffice.Core.Datos.Contratos;
using SATI.BackOffice.Core.Datos.Implementacion;
using SATI.BackOffice.Core.Servicios.Contrato;
using SATI.BackOffice.Core.Servicios.Implementacion;
using SATI.BackOffice.Infraestructura.Helpers;
using SATI.BackOffice.Infraestructura.Intefaces;

namespace SATI.BackOffice.Core.ServicioExtensions
{
    public static class ServicioExtensions
    {
        public static IServiceCollection AddServicios(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<ICarouselServicio, CarouselServicio>();
            services.AddScoped<INormativaLegalServicio, NormativaLegalServicio>();
            services.AddScoped<ITipoNormativaServicio, TipoNormativaServicio>();

            services.AddScoped<ILoggerHelper, LoggerHelper>();

            return services;
        }
    }
}
