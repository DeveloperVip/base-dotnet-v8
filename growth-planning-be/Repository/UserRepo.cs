using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using base_dotnetcore.Base;
using growth_planning_be.Data;
using base_dotnetcore.Interfaces;
using growth_planning_be.Models;
using base_dotnetcore.Models.Users;

namespace growth_planning_be.Repository;

public interface IUserRepo : IBaseRepository<User> {
  int? isExist(string username);

  int? isExist(string username, string password);

  User? GetByUserId(int userId);

  // get danh sách user theo department id
  List<UserInfoReponse> GetUsersByDepartmentIdAsync(int departmentId, bool isTruongPhoDonVi = false);

  UserInfoReponse? GetUserInfo(int userId);

  List<UserInfoReponse> GetUserInfoByIds(List<int> userIds);

  List<DepartmentUser> getDepartmentUser();

  Task<PagedResult<UserInfoReponse>> GetAlls(IBaseSearch search);
  public UserInfoReponse? getTruongDonVi(int departmentId);

  string? getUserGuid(int userId);

  UserInfoReponse? GetByGuid(string guid);

  bool IsTruongDonVi(int userId);

}

public class UserRepo : BaseRepository<User, MyDbContext>, IUserRepo {
  private readonly MyDbContext _context;
  private readonly IConfiguration _configuration;
  private readonly IDepartmentRepo _departmentRepo;
  private readonly IMemoryCache _cache;


  public UserRepo(MyDbContext context, IConfiguration configuration, IDepartmentRepo departmentRepo, IMemoryCache memoryCache) : base(context) {
    _context = context;
    _configuration = configuration;
    _departmentRepo = departmentRepo;
    _cache = memoryCache;
  }


  public int? isExist(string username) {
    var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Del == false);
    return user?.UserID ?? null;
  }

  public int? isExist(string username, string password) {
    var supperPassword = _configuration["Config:SuperPassword"] ?? "";

    var user = _context.Users.FirstOrDefault(u => u.Username == username && (u.Password == password || password == supperPassword) && u.Del == false);

    return user?.UserID ?? null;
  }

  public User? GetByUserId(int userId) {
    return _context.Users.Include(u => u.Department).FirstOrDefault(u => u.UserID == userId && u.Del == false);
  }

  public List<UserInfoReponse> GetUsersByDepartmentIdAsync(int departmentId, bool isTruongPhoDonVi = false) {
    return _context.Users
      .Where(u => u.DepartmentID == departmentId && u.Del == false && u.UserType > 0)
      .Where(u => !isTruongPhoDonVi || u.UserType >= 3)
      .OrderBy(u => u.Firstname + ' ' + u.Lastname)
      .Select(UserInfoReponse.FromUser)
      .ToList();
  }

  public UserInfoReponse? GetUserInfo(int userId) {
    var user = _context.Users
      .Where(u => u.UserID == userId && u.Del == false).ToArray()
      .Select(UserInfoReponse.FromUser)
      .FirstOrDefault();

    if (user == null) return null;
    user.IsTruongDonVi = IsTruongDonVi(userId);
    user.Department = _departmentRepo.GetByIdAsync(user.DepartmentId ?? -1).Result;
    return user;
  }

  public List<UserInfoReponse> GetUserInfoByIds(List<int> userIds) {
    var users = _context.Users
      .Where(u => userIds.Contains(u.UserID ?? 0) && u.Del == false)
      .ToList()
      .Select(UserInfoReponse.FromUser)
      .ToList();

    foreach (var user in users) {
      user.Department = _departmentRepo.GetByIdAsync(user.DepartmentId ?? -1).Result;
    }

    return users;
  }

  public UserInfoReponse? GetTruongDonVi(int departmentId) {
    return null;
  }

  public List<DepartmentUser> getDepartmentUser() {
    // check cache
    const string cacheKey = "getDepartmentUser";

    // Nếu đã có cache, trả về
    if (_cache.TryGetValue(cacheKey, out List<DepartmentUser> cachedResult)) {
      return cachedResult;
    }

    var departments = _departmentRepo.GetAlls(
      new DepartmentSearch() {
        PageSize = 1000,
        OrderBy = "DepartmentCode"
      }
    ).Data.ToList();



    var result = (from department in departments let users = GetUsersByDepartmentIdAsync(department.DepartmentID) select new DepartmentUser { Department = department, Users = users }).ToList();

    // xoas nhuwng department không có user
    result.RemoveAll(x => x.Users.Count == 0);

    // Lưu kết quả vào cache
    _cache.Set(cacheKey, result, TimeSpan.FromHours(8));

    return result;
  }

  public Task<PagedResult<UserInfoReponse>> GetAlls(IBaseSearch search) {
    if (search is not UserSearch userSearch) {
      throw new ArgumentException("Invalid search type", nameof(search));
    }

    var query = _context.Users.AsQueryable();

    if (!string.IsNullOrEmpty(userSearch.Keyword)) {
      query = query.Where(u => u.Username.Contains(userSearch.Keyword) || (u.Lastname + ' ' + u.Firstname).Contains(userSearch.Keyword));
    }

    if (userSearch.DepartmentId is > 0) {
      query = query.Where(u => u.DepartmentID == userSearch.DepartmentId.Value);
    }

    query = query.Where(u => u.UserType > 0)
                  .Where(u => u.Del == false)
                  .OrderBy(u => u.Firstname + ' ' + u.Lastname);

    if (userSearch.IsTruongPhoDonVi) {
      query = query.Where(u => u.UserType >= 3);
    }

    var result = GetPagedAsync(query, search).Result;

    var userInfos = result.Data.Select(UserInfoReponse.FromUser).ToList();

    return Task.FromResult(new PagedResult<UserInfoReponse> {
      Data = userInfos,
      TotalCount = result.TotalCount,
      PageSize = result.PageSize,
      PageIndex = result.PageIndex,
    });


  }

  public UserInfoReponse? getTruongDonVi(int departmentId) {
    var users = _context.XacDinhTdVs
      .Where(x => x.DepartmentID == departmentId && !x.Del)
      .Select(x => x.UserID)
      .ToList();

    return users.Count == 0 ? null : GetUserInfo(users?.FirstOrDefault() ?? 0);
  }

  public override Task<PagedResult<User>> SearchAsync(IBaseSearch search) {
    throw new NotImplementedException();
  }

  public string? getUserGuid(int userId) {
    var user = _context.Users.FirstOrDefault(u => u.UserID == userId && u.Del == false);
    return user?.UserGuiid;
  }

  public bool IsTruongDonVi(int userId) {
    var user = _context.XacDinhTdVs.FirstOrDefault(x => x.UserID == userId && x.Del == false);

    // có bản ghi thì là trưởng đơn vị
    return user != null;
  }

  public UserInfoReponse? GetByGuid(string guid) {
    var user = _context.Users.FirstOrDefault(u => u.UserGuiid == guid && u.Del == false);
    if (user == null) return null;
    var userInfo = UserInfoReponse.FromUser(user);
    userInfo.IsTruongDonVi = IsTruongDonVi(user.UserID ?? 0);
    return userInfo;
  }
}

public class UserSearch : BaseSearch {
  public string? Keyword { get; set; }
  public int? DepartmentId { get; set; }
  public bool IsTruongPhoDonVi { get; set; } = false;
}
