using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Models.Dto;

public class VillaDTO
{
    public int Id { get; set; }
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public int Occupancy { get; set; }
    public int Sqfs { get; set; }
}