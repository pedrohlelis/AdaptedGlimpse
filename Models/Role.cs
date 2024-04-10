using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glimpse.Models;

public class Role
{
    private int _RoleId;
    private String? _RoleName;
    private String? _RoleDescription;
    private String? _RoleColor;
    private bool _CanRemoveMembers;
    private bool _CanCreateRole;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoleId {get{return _RoleId;} set{_RoleId = value;}}
    public String? RoleName {get{return _RoleName;} set{_RoleName = value;}}
    public String? RoleDescription {get{return _RoleDescription;} set{_RoleDescription = value;}}
    public String? RoleColor {get{return _RoleColor;} set{_RoleColor = value;}}
    public bool CanRemoveMembers {get{return _CanRemoveMembers;} set{_CanRemoveMembers = value;}}
    public bool CanCreateRole {get{return _CanCreateRole;} set{_CanCreateRole = value;}}

}
