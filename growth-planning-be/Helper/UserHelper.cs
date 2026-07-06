using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace growth_planning_be.Helper
{
  public class UserHelper
  {
    public static IServiceProvider? serviceProvider;
    public static IConfiguration? _Configuration { get; set; }

    public static void Init(IServiceProvider? _serviceProvider, IConfiguration? Configuration)
    {
      serviceProvider = _serviceProvider;
      _Configuration = Configuration;
    }
    
    public static bool isAdmin
    {
      get
      {
        return false; // TODO: Implement logic to check if the user is an admin
      }
    }
    
    public static int? CurrentUserId
    {
      get
      {
        var httpContext = serviceProvider?.GetService<IHttpContextAccessor>()?.HttpContext;
        if (httpContext == null)
        {
          return null;
        }
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId != null ? int.Parse(userId) : (int?)null;
      }
    }
    
    // applicationId
    public static int ApplicationId => _Configuration?.GetValue<int>("Config:ApplicationID") ?? 0;
    
    // langCode
    public static string LangCode
    {
      get
      {
        return "vi";
        // var httpContext = serviceProvider?.GetService<IHttpContextAccessor>()?.HttpContext;
        // if (httpContext == null)
        // {
        //   return "vi";
        // }
        // var langCode = httpContext.Request.Headers.AcceptLanguage.ToString();
        // return string.IsNullOrEmpty(langCode) ? "vi" : langCode;
      }
    }
  }
}
