using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NuGet.Protocol.Core.Types;

namespace Glimpse.Models;

public class Project 
{
    [Key]
    public int Id { get; set; }
    public string? ResponsibleUserId { get; set; }
    public string? Name { get; set; }
    public DateOnly CreationDate { get; set; }
    public DateTime LastEdited { get; set; }
    public string? Description { get; set; }
    public string? Picture { get; set; }
    public bool IsActive { get; set; }
    public virtual List<Repository> Repositories { get; set; } = [];
    public virtual List<Board> Boards { get; set; } = [];
    public virtual List<Role> Roles { get; set; } = [];
    public virtual List<User> Users { get; set; } = [];

    public static implicit operator Project(ValueTask<Project?> v)
    {
        throw new NotImplementedException();
    }
}