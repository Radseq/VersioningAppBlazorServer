using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFDataAccessLib.Models;

[Index(nameof(ApplicationId), nameof(Major), nameof(Minor), nameof(Patch), IsUnique = true)]
public class AppVersion
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int Major { get; set; }

    [Required]
    public int Minor { get; set; }

    [Required]
    public int Patch { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = null!;

    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;

    //public IList<AppCompatibility> AppCompatibilitySourceVersions { get; set; } = [];

    //public IList<AppCompatibility> AppCompatibilityTargetVersions { get; set; } = [];
    public IList<AppCompatibility> CompatibilitySourceVersions { get; set; } = new List<AppCompatibility>();
    public IList<AppCompatibility> CompatibilityTargetVersions { get; set; } = new List<AppCompatibility>();

}
