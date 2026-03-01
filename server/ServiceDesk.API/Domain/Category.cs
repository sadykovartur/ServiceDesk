using System.ComponentModel.DataAnnotations;

namespace ServiceDesk.API.Domain;

public class Category
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
