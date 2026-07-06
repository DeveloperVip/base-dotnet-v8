using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace growth_planning_be.Attributes;

public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter {
  public string? Role { get; set; }
  public string? LevelExpr { get; set; }

  public void OnAuthorization(AuthorizationFilterContext context) {
    var user = context.HttpContext.User;
    if (user?.Identity == null || !user.Identity.IsAuthenticated) {
      context.Result = new UnauthorizedResult();
      return;
    }

    var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    var levelClaim = user.FindFirst("Level")?.Value;
    int.TryParse(levelClaim, out var userLevel);

    var roleMatch = false;
    if (!string.IsNullOrWhiteSpace(Role)) {
      var allowed = Role.Split(',').Select(r => r.Trim());
      roleMatch = roles.Intersect(allowed).Any();
    }

    var levelMatch = false;
    if (!string.IsNullOrWhiteSpace(LevelExpr)) {
      levelMatch = EvaluateLevel(userLevel, LevelExpr);
    }

    if (!(roleMatch || levelMatch)) {
      context.Result = new ForbidResult();
    }
  }

  private static bool EvaluateLevel(int level, string expr) {
    expr = expr.Trim();
    if (expr.StartsWith(">=")) return level >= int.Parse(expr[2..]);
    if (expr.StartsWith("<=")) return level <= int.Parse(expr[2..]);
    if (expr.StartsWith(">")) return level > int.Parse(expr[1..]);
    if (expr.StartsWith("<")) return level < int.Parse(expr[1..]);
    if (expr.StartsWith("=")) return level == int.Parse(expr[1..]);
    return int.TryParse(expr, out var val) && level == val;
  }
}
