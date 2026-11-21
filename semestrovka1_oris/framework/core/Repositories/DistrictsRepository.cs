using ORM;
using semestrovka1_oris.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestrovka1_oris.framework.core.Repositories
{
    public class DistrictsRepository
    {
        private IOrmContext orm;
        public DistrictsRepository(IOrmContext orm)
        {
            this.orm = orm;
        }
        public async Task<IReadOnlyList<District>> GetAllAsync()
        {
            return await orm.GetAllAsync<District>("Districts");
        }
        public async Task<int> DeleteAsync(int id)
        {
            return await orm.DeleteAsync("Districts", id);
        }
        public async Task<int> UpdateAsync(int id, District district)
        {
            return await orm.UpdateAsync<District>("Districts", id, district);
        }
        public async Task<int> InsertAsync(District district)
        {
            return await orm.InsertAsync<District>("Districts", district);
        }
    }
}
