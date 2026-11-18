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
        [HttpGET]
        public (string, int) Rules(NameValueCollection query)
        {
            string contentHtml = File.ReadAllText("static/templates/rules.html");
            string layout = File.ReadAllText("static/layout.html");
            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200);
        }
        [HttpGET]
        public (string, int) Search(NameValueCollection query)
        {
            using (var db = new GameContext()) 
            {
                string searchReq = (query["q"] ?? "").Trim();
                
                var foundRoles = db.Roles.ToList().Where(r => r.Name.Contains(searchReq, StringComparison.OrdinalIgnoreCase) || r.Description.Contains(searchReq, StringComparison.OrdinalIgnoreCase));
                var foundDistricts = db.Districts.ToList().Where(d => d.Name.Contains(searchReq, StringComparison.OrdinalIgnoreCase) || d.Description.Contains(searchReq, StringComparison.OrdinalIgnoreCase));
                var dataModel = new { roles = foundRoles, districts = foundDistricts };

                HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
                string template = File.ReadAllText("static/templates/search.html");
                string contentHtml1 = renderer.RenderFromString4(template, dataModel);
                string contentHtml2 = renderer.RenderFromString4(contentHtml1, dataModel);
                switch (searchReq)
                {
                    case "кчау":
                    case "kchau":
                        contentHtml2 = "<img src=/images/kchau.jpg>"; break;
                    case "aphex twin":
                    case "афекс твин":
                    case "windowlicker":
                        contentHtml2 = "<img src=/images/aphextwin.jpg>"; break;
                    case "pudge":
                    case "пудж":
                        contentHtml2 = "<img src=/images/pudge.jpg>"; break;
                    case "скачать армяне":
                        contentHtml2 = "<img src=/images/download_armanians.jpg>"; break;
                    case "порно":
                        contentHtml2 = "<img src=/images/not_today.jpg>"; break;
                }

                string layout = File.ReadAllText("static/layout.html");
                string result = layout.Replace("{{Content}}", contentHtml2);
                
                return (result, 200);
            }
        }
    }
}
