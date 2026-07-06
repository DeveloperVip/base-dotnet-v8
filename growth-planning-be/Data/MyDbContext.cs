using base_dotnetcore.Models.Department;
using base_dotnetcore.Models.Users;
using Microsoft.EntityFrameworkCore;
using growth_planning_be.Models.Function;

namespace growth_planning_be.Data;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options) {
  public DbSet<User> Users { get; set; }
  public DbSet<XacDinhTDVs> XacDinhTdVs { get; set; }

  // permission
  public DbSet<Function> Functions { get; set; }
  public DbSet<FunctionUser> FunctionUsers { get; set; }
  public DbSet<GroupFunction> GroupFunctions { get; set; }
  public DbSet<FunctionDto> FunctionDtos { get; set; }
  public DbSet<Department> Departments { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<User>().ToTable("vnk_User");
    modelBuilder.Entity<FunctionDto>().HasNoKey();
  }
}
