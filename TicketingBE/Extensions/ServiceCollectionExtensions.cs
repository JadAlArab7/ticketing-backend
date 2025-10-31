using TicketingBE.BLL.Interfaces;
using TicketingBE.BLL.Services;
using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.DAL.Repositories;

namespace TicketingBE.Extensions
{
    /// <summary>
    /// Extension methods for registering application services in dependency injection container
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register all Data Access Layer (DAL) repositories
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services)
        {
            // Register SqlHelper for database operations
            services.AddSingleton<SqlHelper>();
            
            // Register all repository interfaces and their implementations
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            
            // Add more repositories here as your application grows
            // services.AddScoped<ITicketRepository, TicketRepository>();
            // services.AddScoped<ICommentRepository, CommentRepository>();
            
            return services;
        }

        /// <summary>
        /// Register all Business Logic Layer (BLL) services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            // Register all service interfaces and their implementations
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            
            // Add more services here as your application grows
            // services.AddScoped<ITicketService, TicketService>();
            // services.AddScoped<ICommentService, CommentService>();
            
            return services;
        }
    }
}
