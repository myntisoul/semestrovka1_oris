using ORM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using semestrovka1_oris.models;
using System.ComponentModel.DataAnnotations;

namespace semestrovka1_oris.framework.core
{
    [HttpController("auth")]
    public class AuthController
    {
        OrmContext orm = new OrmContext("Server=localhost,1433;Database=OrisDb;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;");
        [HttpPOST]
        public async Task<(string, int, string)> Register(NameValueCollection query)
        {
            string password = query["password"] ?? "";
            string login = query["login"] ?? "";
            string email = query["email"] ?? "";

            UsersRepository rep = new UsersRepository(orm);
            await rep.InsertAsync(new User() { Email = email, Login = login, Password = password });

            Console.WriteLine(login + " " + email + " " + "password");
            foreach (User user in await rep.GetAllAsync()) Console.WriteLine(user.Login);

            return ("", 303, "/");
        }
    }
}
