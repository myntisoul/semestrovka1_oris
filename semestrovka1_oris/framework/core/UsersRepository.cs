using ORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using semestrovka1_oris.models;

namespace semestrovka1_oris.framework.core
{
    public class UsersRepository
    {
        private IOrmContext orm;
        public UsersRepository(IOrmContext orm)
        {
            this.orm = orm;
        }
        public async Task<int> InsertAsync(User user)
        {
            return await orm.InsertAsync<User>("UsersWiki", user);
        }
        public async Task<IReadOnlyList<User>> GetAllAsync()
        {
            return await orm.GetAllAsync<User>("UsersWiki");
        }

    }
}
