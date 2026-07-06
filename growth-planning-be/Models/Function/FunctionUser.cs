

using System.ComponentModel.DataAnnotations;
using base_dotnetcore.Base;

namespace growth_planning_be.Models.Function;

public class FunctionUser : BaseModel {
  [Key]
  public int? FunctionID { get; set; }
  public int? UserID { get; set; }
}
