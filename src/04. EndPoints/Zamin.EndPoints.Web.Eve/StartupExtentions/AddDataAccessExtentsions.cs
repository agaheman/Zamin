using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Zamin.Core.Contracts.Eve.Data.Commands;
using Zamin.Core.Contracts.Eve.Data.Queries;

namespace Zamin.EndPoints.Web.Eve.StartupExtentions
{
    public static class AddDataAccessExtentsions
    {

        public static IServiceCollection AddEveDataAccess(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
            => services.AddEveRepositories(assembliesForSearch);

        public static IServiceCollection AddEveRepositories(this IServiceCollection services, IEnumerable<Assembly> assembliesForSearch)
            => services.AddWithTransientLifetime(assembliesForSearch, typeof(IEveCommandRepository<>), typeof(IEveQueryRepository<>));
    }
}