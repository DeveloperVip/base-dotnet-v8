using base_dotnetcore.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using growth_planning_be.Models.Auth;
using growth_planning_be.Services;

namespace growth_planning_be.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController(IAuthService authService) : ControllerBase {
  [HttpPost]
  public BaseResult<LoginResponse> Login([FromBody] LoginRequestModel loginRequest) {
    var res = authService.Login(loginRequest);
    return res == null ? BaseResult<LoginResponse>.Error("Invalid username or password.") : new BaseResult<LoginResponse>(res);
  }

  [HttpGet]
  [Authorize]
  public IActionResult HealthCheck() {
    return Ok("Service is running");
  }

  [HttpGet]
  public BaseResult<LoginResponse> LoginSSO(string token) {
    var res = authService.LoginSSO(token);
    return res == null ? BaseResult<LoginResponse>.Error("Invalid token.") : new BaseResult<LoginResponse>(res);
  }

  [HttpGet]
  public IActionResult HealCheck() {
    return Ok("Heal check OK");
  }
}
