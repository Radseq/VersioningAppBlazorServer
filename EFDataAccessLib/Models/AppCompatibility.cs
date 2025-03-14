using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFDataAccessLib.Models;

[Index(nameof(SourceVersionId), nameof(TargetVersionId), IsUnique = true)]
public class AppCompatibility
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [ForeignKey(nameof(SourceVersion))]
    public int SourceVersionId { get; set; }
    public AppVersion SourceVersion { get; set; } = null!;

    [ForeignKey(nameof(TargetVersion))]
    public int TargetVersionId { get; set; }
    public AppVersion TargetVersion { get; set; } = null!;
}
