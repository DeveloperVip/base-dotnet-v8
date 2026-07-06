using base_dotnetcore.Base;
using base_dotnetcore.Interfaces;
using growth_planning_be.Data;
using growth_planning_be.Repository;
using growth_planning_be.Services;

namespace growth_planning_be.Extensions;

public static class ServiceCollectionExtensions {
  public static IServiceCollection AddRepositories(this IServiceCollection services) {
    services.AddScoped<MyDbContext>();

    services.AddScoped<IPermissionRepo, PermissionRepo>();
    services.AddScoped<IUserRepo, UserRepo>();
    services.AddScoped<IDepartmentRepo, DepartmentRepo>();

    services.AddSingleton<TelegramService>();
    services.AddScoped<IAuthService, AuthService>();

    return services;
  }
}
