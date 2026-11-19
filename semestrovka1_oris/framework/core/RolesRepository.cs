using ORM;
using semestrovka1_oris.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestrovka1_oris.framework.core
{
    public class RolesRepository
    {
        private IOrmContext orm;
        public RolesRepository(IOrmContext orm)
        {
            this.orm = orm;
        }
        public async Task<IReadOnlyList<Role>> GetAllAsync()
        {
            return await orm.GetAllAsync<Role>("Roles");
        }
    }
}
