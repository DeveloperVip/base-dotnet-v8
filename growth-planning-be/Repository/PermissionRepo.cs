using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using growth_planning_be.Data;
using growth_planning_be.Models.Function;

namespace growth_planning_be.Repository;

public interface IPermissionRepo {
  Task<List<FunctionDto>> GetFuntions(int userId, int applicationId);
}

public class PermissionRepo(MyDbContext context) : IPermissionRepo {
  public async Task<List<FunctionDto>> GetFuntions(int userId, int applicationId) {
    var sql = @"
        IF @UserID > 0
        BEGIN
            ;WITH A AS (
                SELECT f.FunctionID, f.ParentID, f.Code, f.Name
                FROM [Function] f
                WHERE f.Del = 0 AND
                    (
                        f.FunctionID IN (
                            SELECT gf.FunctionID
                            FROM GroupFunction gf
                            WHERE gf.GroupID IN (
                                SELECT gu.GroupID
                                FROM GroupUser gu
                                WHERE gu.UserID = @UserID
                            )
                        )
                        OR f.FunctionID IN (
                            SELECT fu.FunctionID
                            FROM FunctionUser fu
                            WHERE fu.UserID = @UserID AND fu.ApplicationID = @ApplicationID
                        )
                    )

                UNION ALL

                SELECT c.FunctionID, c.ParentID, c.Code, c.Name
                FROM [Function] c
                INNER JOIN A ON c.ParentID = A.FunctionID
                WHERE c.Del = 0
            )

            SELECT A.FunctionID, A.Code, A.Name
            FROM A

            UNION

            SELECT -1 AS FunctionID,
                   CASE WHEN u.UserType > 2 THEN 'truong-khoa' ELSE 'giao-vien' END AS Code,
                   'Vai trò hệ thống' AS Name
            FROM vnk_User u
            WHERE u.Del = 0 AND u.UserID = @UserID AND u.UserType >= 1
        END
        ELSE
        BEGIN
            SELECT -1 AS FunctionID, '' AS Code, '' AS Name
        END";

    var userParam = new SqlParameter("@UserID", userId);
    var appParam = new SqlParameter("@ApplicationID", applicationId);

    return await context.FunctionDtos
        .FromSqlRaw(sql, userParam, appParam)
        .ToListAsync();
  }
}