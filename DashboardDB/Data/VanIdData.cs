using DBLibrary.DbAccess;
using DBLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Data
{
    public class VanIdData : IVanIdData
    {
        private readonly ISqlDataAccess _db;

        public VanIdData(ISqlDataAccess db)
        {
            _db = db;
        }

        public Task<IEnumerable<VanID>> GetIds()
            => _db.LoadData<VanID, dynamic>("dbo.spVanId_GetAll", new { });

        public async Task<VanID?> GetId(string vanName)
        {
            var results = await _db.LoadData<VanID, dynamic>("dbo.spVanId_get", new { VanName = vanName });

            return results.FirstOrDefault();
        }

        public async Task InsertVanId(VanID vanId)
            => await _db.SaveData("dbo.spVanId_Insert", vanId);

        public async Task UpdateVanId(VanID vanId)
            => await _db.SaveData("dbo.spVanId_Update", vanId);

        public async Task DeleteVanId(string vanName)
            => await _db.SaveData("dbo.SpVanId_Delete", new { VanName = vanName });
        
        public async Task BlockVan(string vanName, bool blocked)
            => await _db.SaveData("dbo.SpVanId_Block", new { VanName = vanName, Blocked = blocked });
    }
}
