using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EFDataAccessLib.Models;

[Index(nameof(Name), IsUnique = true)]
public class Application
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(200)]
    public string? Description { get; set; }

    public IList<AppVersion> AppVersions { get; set; } = [];
}
