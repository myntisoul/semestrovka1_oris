using ORM;
using semestrovka1_oris.framework.core.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace semestrovka1_oris.framework.core.Controllers
{
    [HttpController("wiki")]
    public class WikiController
    {
        OrmContext orm = WikiDatabase.Connect();
        public HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
        string layout = File.ReadAllText("static/layout.html");

        [HttpGET]
        public async Task<(string, int)> Roles(NameValueCollection query)
        {
            string template = File.ReadAllText("static/templates/roles.html");
            //var roles = await orm.GetAllAsync<Role>("Roles");
            RolesRepository rep = new RolesRepository(orm);
            var roles = await rep.GetAllAsync();
            var dataModel = new { roles };
            string contentHtml = renderer.RenderFromString4(template, dataModel);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }
        [HttpGET]
        public async Task<(string, int)> RolesDetails(NameValueCollection query)
        {
            if (!int.TryParse(query["id"], out int id)) return ("400", 400);
            RolesRepository rep = new RolesRepository(orm);
            var roles = await rep.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return ("404", 404);

            string template = File.ReadAllText("static/templates/role_details.html");
            string contentHtml = renderer.RenderFromString(template, role);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }

        [HttpGET]
        public async Task<(string, int)> Districts(NameValueCollection query)
        {
            DistrictsRepository rep = new DistrictsRepository(orm);
            var districts = await rep.GetAllAsync();
            string template = File.ReadAllText("static/templates/districts.html");
            var dataModel = new { districts };
            string contentHtml = renderer.RenderFromString4(template, dataModel);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }

        [HttpGET]
        public async Task<(string, int)> DistrictsDetails(NameValueCollection query)
        {
            if (!int.TryParse(query["id"], out int id)) return ("400", 400);
            DistrictsRepository rep = new DistrictsRepository(orm);
            var districts = await rep.GetAllAsync();
            var district = districts.FirstOrDefault(d => d.Id == id);
            if (district == null) return ("404", 404);
            string template = File.ReadAllText("static/templates/district_details.html");

            BonusText bonusText = new BonusText();
            switch (district.Color)
            {
                case "Зеленый": bonusText.Text = "Торговец получает одну дополнительную монету за эту карту."; break;
                case "Желтый": bonusText.Text = "Король получает одну дополнительную монету за эту карту."; break;
                case "Синий": bonusText.Text = "Епископ получает одну дополнительную монету за эту карту."; break;
                case "Красный": bonusText.Text = "Военачальник получает одну дополнительную монету за эту карту."; break;
            }

            string contentHtml = renderer.RenderFromString(template, district);
            contentHtml = renderer.RenderFromString(contentHtml, bonusText);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }
        [HttpGET]
        public (string, int) Rules(NameValueCollection query)
        {
            string contentHtml = File.ReadAllText("static/templates/rules.html");
            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }
        [HttpGET]
        public async Task<(string, int)> Search(NameValueCollection query)
        {
            string searchReq = (query["q"] ?? "").Trim();

            DistrictsRepository repDistricts = new DistrictsRepository(orm);
            var districts = await repDistricts.GetAllAsync();
            RolesRepository repRoles = new RolesRepository(orm);
            var roles = await repRoles.GetAllAsync();

            var foundRoles = roles
                .Where(role => role.Name.Contains(searchReq, StringComparison.OrdinalIgnoreCase) ||
                               role.Description.Contains(searchReq, StringComparison.OrdinalIgnoreCase))
                .Select(role => new { role.Id, role.TurnOrder, Name = WebUtility.HtmlEncode(role.Name) })
                .ToArray();
            var foundDistricts = districts
                .Where(district => district.Name.Contains(searchReq, StringComparison.OrdinalIgnoreCase) ||
                                   district.Description.Contains(searchReq, StringComparison.OrdinalIgnoreCase))
                .Select(district => new { district.Id, Name = WebUtility.HtmlEncode(district.Name) })
                .ToArray();
            var dataModel = new { roles = foundRoles, districts = foundDistricts };

            string template = File.ReadAllText("static/templates/search.html");
            string contentHtml = foundRoles.Length == 0 && foundDistricts.Length == 0
                ? "<h2>Результаты поиска</h2><p>Ничего не найдено.</p>"
                : renderer.RenderFromString4(template, dataModel);
            switch (searchReq)
            {
                case "pudge":
                case "пудж":
                    contentHtml = "<img src=/images/pudge.jpg>"; break;
                case "скачать армяне":
                    contentHtml = "<img src=/images/download_armanians.jpg>"; break;
            }

            return query["fragment"] == "1"
                ? (contentHtml, 200)
                : (layout.Replace("{{Content}}", contentHtml), 200);
        }
        [HttpGET]
        public async Task<(string, int)> Login(NameValueCollection query)
        {
            string contentHtml = File.ReadAllText("static/login.html");
            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }
        [HttpGET]
        public async Task<(string, int, string)> WrongLogin(NameValueCollection query)
        {
            string content = File.ReadAllText("static/wrong_login.html");
            string result = layout.Replace("{{Content}}", content);
            return (result, 200, "");
        }
        [HttpGET]
        public async Task<(string, int, string)> Register(NameValueCollection query)
        {
            string content = File.ReadAllText("static/register.html");
            string result = layout.Replace("{{Content}}", content);
            return (result, 200, "");
        }
    }
}
