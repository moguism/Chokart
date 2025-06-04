namespace server.Models.DTOs;

public class ModifyRoleRequest
{
    public int UserId { get; set; }
    public string NewRole{ get; set; }
}
