using System.Security.Claims;
using base_dotnetcore.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using base_dotnetcore.Models.Users;
using growth_planning_be.Repository;
using growth_planning_be.Services;

namespace growth_planning_be.Controllers.v1;

[ApiController]
[Authorize]
[Route("api/v1/[controller]/[action]")]
public class UserController(IAuthService authService) : ControllerBase {
  [HttpGet]
  public BaseResult<UserInfoReponse> GetUserInfo() {
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var userInfo = authService.GetUserInfo(int.TryParse(userId, out var id) ? id : 0);
    return userInfo == null ? BaseResult<UserInfoReponse>.Error("User not found.") : new BaseResult<UserInfoReponse>(userInfo);
  }
}
