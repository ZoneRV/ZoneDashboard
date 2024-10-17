using DBLibrary.Models;

namespace DBLibrary.Data
{
    public interface IVanIdData
    {
        Task DeleteVanId(string vanName);
        Task<VanID?> GetId(string vanName);
        Task<IEnumerable<VanID>> GetIds();
        Task InsertVanId(VanID vanId);
        Task UpdateVanId(VanID vanId);
        Task BlockVan(string vanName, bool blocked);
    }
}