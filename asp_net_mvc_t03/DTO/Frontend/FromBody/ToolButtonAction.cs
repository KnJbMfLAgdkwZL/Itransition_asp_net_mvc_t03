using System.ComponentModel.DataAnnotations;

namespace asp_net_mvc_t03.DTO.Frontend.FromBody;

public class ToolButtonAction
{
    [Required] public string Action { get; set; } = string.Empty;

    [Required] public List<int> UsersId { get; set; } = new List<int>();
}