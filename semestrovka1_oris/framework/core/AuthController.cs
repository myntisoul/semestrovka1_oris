using ORM;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using semestrovka1_oris.models;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace semestrovka1_oris.framework.core
{
    [HttpController("auth")]
    public class AuthController
    {
        string layout = File.ReadAllText("static/layout.html");
        OrmContext orm = new OrmContext("Server=localhost,1433;Database=OrisDb;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;");
        
        [HttpPOST]
        public async Task<(string, int, string)> Register(NameValueCollection query)
        {
            string password = query["password"] ?? "";
            string login = query["login"] ?? "";
            string email = query["email"] ?? "";

            UsersRepository rep = new UsersRepository(orm);
            await rep.InsertAsync(new User() { Email = email, Login = login, Password = password });

            Console.WriteLine(login + " " + email + " " + password);
            foreach (User user in await rep.GetAllAsync()) Console.WriteLine(user.Login);

            return ("", 303, "/");
        }

        [HttpPOST]
        public async Task<(string, int, string)> Login(NameValueCollection query)
        {
            string password = query["password"] ?? "";
            string login = query["login"] ?? "";
            string email = query["email"] ?? "";

            UsersRepository rep = new UsersRepository(orm);
            var users = await rep.GetAllAsync();
            if (users.FirstOrDefault(u => u.Login == login && u.Email == email && u.Password == password) != null)
            {
                return ("", 303, "/");
            }
            else
            {
                string loginForm = File.ReadAllText("static/login.html");
                //StringBuilder sb = new StringBuilder();
                //sb.Append(loginForm);
                //sb.Append("<p>Аккаунт не найден. Возможно, вы ошиблись в своих данных.</p>");
                return ("", 303, "/wiki/wronglogin");
            }
        }
    }
}
