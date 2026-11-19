using ORM;
using semestrovka1_oris.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestrovka1_oris.framework.core
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
    }
}
