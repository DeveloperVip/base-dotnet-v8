using growth_planning_be.Models.Function;
using growth_planning_be.Repository;

namespace growth_planning_be.Services;

public interface IPermissionService
{
    Task<List<FunctionDto>> GetFuntions(int userId, int applicationId);

}


public class PermissionService(PermissionRepo permissionRepo) : IPermissionService
{
    public Task<List<FunctionDto>> GetFuntions(int userId, int applicationId)
    {
        return permissionRepo.GetFuntions(userId, applicationId);
    }
}