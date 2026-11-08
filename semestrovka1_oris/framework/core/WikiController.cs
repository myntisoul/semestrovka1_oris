using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace semestrovka1_oris.framework.core
{
    [HttpController("wiki")]
    public class WikiController
    {
        public HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();

        [HttpGET]
        public (string, int) Roles(NameValueCollection query)
        {
            using (var db = new GameContext())
            {
                string template = File.ReadAllText("static/templates/roles.html");
                var roles = db.Roles.ToList();
                var dataModel = new { roles = roles };
                string contentHtml = renderer.RenderFromString4(template, dataModel);

                string layout = File.ReadAllText("static/layout.html");
                string result = layout.Replace("{{Content}}", contentHtml);
                return (result, 200);
            }    
        }
        [HttpGET]
        public (string, int) RolesDetails(NameValueCollection query)
        {
            using (var db = new GameContext())
            {
                if (!int.TryParse(query["id"], out int id)) return ("400", 400);

                var role = db.Roles.FirstOrDefault(r => r.Id == id);
                if (role == null) return ("404", 404);

                string template = File.ReadAllText("static/templates/role_details.html");
                string contentHtml = renderer.RenderFromString(template, role);

                string layout = File.ReadAllText("static/layout.html");
                string result = layout.Replace("{{Content}}", contentHtml);
                return (result, 200);
            }
        }

        [HttpGET]
        public (string, int) Districts(NameValueCollection query)
        {
            using (var db = new GameContext())
            {
                string template = File.ReadAllText("static/templates/districts.html");
                var districts = db.Districts.ToList();
                var dataModel = new { districts = districts };
                string contentHtml = renderer.RenderFromString4(template, dataModel);

                string layout = File.ReadAllText("static/layout.html");
                string result = layout.Replace("{{Content}}", contentHtml);
                return (result, 200);
            }
        }

        [HttpGET]
        public (string, int) DistrictsDetails(NameValueCollection query)
        {
            using (var db = new GameContext())
            {
                if (!int.TryParse(query["id"], out int id)) return ("400", 400);

                var district = db.Districts.FirstOrDefault(d => d.Id == id);
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

                string layout = File.ReadAllText("static/layout.html");
                string result = layout.Replace("{{Content}}", contentHtml);
                return (result, 200);
            }
        }
    }
}
