using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Caching.Memory;
using base_dotnetcore.Base;
using growth_planning_be.Data;
using growth_planning_be.Models;
using base_dotnetcore.Interfaces;
using base_dotnetcore.Models.Department;

namespace growth_planning_be.Repository;

public interface IDepartmentRepo : IBaseRepository<Department> {
  PagedResult<DepartmentReponse> GetAlls(DepartmentSearch search);

  List<DepartmentReponse> GetByIds(List<int>? ids);

  DepartmentReponse? GetById(int id);

  // get list department + user

}

public class DepartmentRepo : BaseRepository<Department, MyDbContext>, IDepartmentRepo {


  public DepartmentRepo(MyDbContext context) : base(context) {
  }

  public PagedResult<DepartmentReponse> GetAlls(DepartmentSearch search) {
    var query = _context.Departments.AsQueryable();

    query = query.Where(x => x.Del == false && x.Active == true);

    if (search.DepartmentTypeID.HasValue) {
      query = query.Where(x => x.DepartmentTypeID == search.DepartmentTypeID.Value);
    }

    if (!string.IsNullOrEmpty(search.Keyword)) {
      query = query.Where(x => x.DepartmentName!.Contains(search.Keyword) || x.DepartmentCode!.Contains(search.Keyword));
    }

    var pagedResult = GetPagedAsync(query, search).Result;
    var data = pagedResult.Data.Select(x => new DepartmentReponse(x));

    return new PagedResult<DepartmentReponse>() {
      Data = data,
      PageIndex = pagedResult.PageIndex,
      PageSize = pagedResult.PageSize,
      TotalCount = pagedResult.TotalCount,
    };

  }

  public List<DepartmentReponse> GetByIds(List<int>? ids) {
    var query = _context.Departments.AsQueryable();
    ids ??= [];
    query = query.Where(x => ids.Contains(x.DepartmentID) && x.Del == false);


    return query.Select(x => new DepartmentReponse(x)).ToList();
  }

  public DepartmentReponse? GetById(int id) {
    var query = _context.Departments.AsQueryable();
    query = query.Where(x => x.DepartmentID == id && x.Del == false);
    return query.Select(x => new DepartmentReponse(x)).FirstOrDefault();
  }
}

public class DepartmentSearch : BaseSearch {
  public int? DepartmentTypeID { get; set; }
}
