using System.ComponentModel.DataAnnotations;
using base_dotnetcore.Base;

namespace growth_planning_be.Models.Function;

public class Function : BaseModel {
  [Key]
  public int? FunctionID { get; set; }
  public string? CategoryCode { get; set; }
  public string? Code { get; set; }
  public string? Name { get; set; }
  public int? ParentID { get; set; }
}