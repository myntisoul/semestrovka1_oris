using ORM;
using semestrovka1_oris.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestrovka1_oris.framework.core.Repositories
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
            var roles = await orm.GetAllAsync<Role>("Roles");
            return roles.OrderBy(role => role.TurnOrder).ThenBy(role => role.Id).ToArray();
        }
        public async Task<int> DeleteAsync(int id)
        {
            return await orm.DeleteAsync("Roles", id);
        }
        public async Task<int> UpdateAsync(int id, Role role)
        {
            return await orm.UpdateAsync<Role>("Roles", id, role);
        }
        public async Task<int> InsertAsync(Role role)
        {
            return await orm.InsertAsync<Role>("Roles", role);
        }
    }
}
