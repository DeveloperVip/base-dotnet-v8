using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using growth_planning_be.Attributes;
using growth_planning_be.Data;
using growth_planning_be.Helper;
using growth_planning_be.Models.Auth;
using base_dotnetcore.Models.Users;
using growth_planning_be.Repository;

namespace growth_planning_be.Services;

public interface IAuthService {
  // Login
  LoginResponse? Login(LoginRequestModel model);

  // create token
  string CreateToken(int userId);

  // get user info
  UserInfoReponse? GetUserInfo(int userId);

  // Logout
  void Logout();

  // loginSSO
  LoginResponse? LoginSSO(string token);
}

public class AuthService(IUserRepo userRepo, IConfiguration configuration, IPermissionRepo permissionRepo, MyDbContext context)
    : IAuthService {
  private readonly IPermissionRepo _permissionRepo = permissionRepo;
  private readonly MyDbContext _context = context;

  public LoginResponse? Login(LoginRequestModel model) {
    // convet md5 password
    model.Password = HashHelper.ConvertToMD5(model.Password);

    // check user exist
    var userId = userRepo.isExist(model.Username, model.Password);
    if (userId == null) {
      return null; // Invalid username or password
    }

    var user = userRepo.GetByUserId(userId.Value);

    // get list functions
    var listFunctions = _permissionRepo.GetFuntions(userId ?? 0, 1).Result;

    string roles = string.Join(",", listFunctions.Select(f => f.Code));

    var tokenString = CreateToken(userId.Value);

    // Return the token and roles
    return new LoginResponse {
      Token = tokenString,
      Roles = listFunctions.Select(f => f.Code).ToList()
    };

  }

  public string CreateToken(int userId) {
    var user = userRepo.GetByUserId(userId);

    // Create JWT token
    var claims = new[]
    {
            new Claim(ClaimTypes.Name, user?.Username ?? ""),
            new Claim(ClaimTypes.Role, ""), // TODO: Set roles properly
            new Claim(ClaimTypes.NameIdentifier, userId.ToString() ?? "0"),
            new Claim("Level", user?.LevelID.ToString() ?? "0"),
            new Claim("UserGuid", user?.UserGuiid ?? ""),
        };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: configuration["Jwt:Issuer"],
        audience: configuration["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds);

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    return tokenString;
  }

  public UserInfoReponse? GetUserInfo(int userId) {
    return userRepo.GetUserInfo(userId);
  }
  public void Logout() {
    throw new NotImplementedException();
  }

  public string RefreshToken() {
    throw new NotImplementedException();
  }

  public LoginResponse? LoginSSO(string token) {
    int UserID = -1;
    var user = GetUserLogin(token);
    if (user.Count() == 1) {
      UserID = user.First().UserID;
    }

    // if user not exist
    if (UserID == -1) {
      throw new Exception("Người dùng không tồn tại");
    }

    // get list functions
    var listFunctions = _permissionRepo.GetFuntions(UserID, 1).Result;

    string roles = string.Join(",", listFunctions.Select(f => f.Code));

    var tokenString = CreateToken(UserID);

    // Return the token and roles
    return new LoginResponse {
      Token = tokenString,
      Roles = listFunctions.Select(f => f.Code).ToList()
    };

  }

  private IQueryable<UserLoginDto> GetUserLogin(string Code) {
    string SQL = @"Select vnk_User.UserID
     , vnk_User.Username
     , (vnk_User.Lastname + ' ' + vnk_User.Firstname) As Fullname
     , IsAdmin
     , UserType
     , vnk_User.Lastname
     , vnk_User.Firstname
     , vnk_User.Images
     , vnk_User.DepartmentID
     , vnk_Department.DepartmentName
     , vnk_Department.DepartmentCode
from LoginGuid
         Left Join vnk_User On LoginGuid.UserID = vnk_User.UserID
         Left Join vnk_Department ON vnk_User.DepartmentID = vnk_Department.DepartmentID
where LoginGuid.Level > 0
  AND LoginGuid.Del = 0
  And vnk_User.Del = 0
  And LoginGuid = @Guid
  And LoginGuid.CreatedTime <= GetDate()
  And LoginGuid.ExpriceTime >= GetDate()";


    var user = _context.Database.SqlQueryRaw<UserLoginDto>(SQL, new SqlParameter("Guid", Code));
    return user;
  }
}

public class LoginRequestModel {
  public string Username { get; set; }
  public string Password { get; set; }
}

public class UserLoginDto {
  public int UserID { get; set; }
  public string? Username { get; set; }
  public string? Fullname { get; set; }
  public bool? IsAdmin { get; set; }
  public int? UserType { get; set; }
  public string? Lastname { get; set; }
  public string? Firstname { get; set; }
  public string? Images { get; set; }
  public int? DepartmentID { get; set; }
  public string? DepartmentName { get; set; }
  public string? DepartmentCode { get; set; }
}