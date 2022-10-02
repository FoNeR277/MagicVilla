using MagicVilla_API.Models.Dto;

namespace MagicVilla_API.Data;

public static class VillaStore
{
    public static List<VillaDTO?> VillaList = new List<VillaDTO?>
    {
        new VillaDTO { Id = 1, Name = "Pool View" , Sqfs = 100, Occupancy = 4},
        new VillaDTO { Id = 2, Name = "Beach View", Sqfs = 200, Occupancy = 3}
    };
}