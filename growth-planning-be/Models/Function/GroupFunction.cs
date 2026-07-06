using System.ComponentModel.DataAnnotations;

namespace growth_planning_be.Models.Function;

public class GroupFunction {
  [Key]
  public int? GroupID { get; set; }
  public int? FunctionID { get; set; }
}