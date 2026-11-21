using HttpMultipartParser;
using ORM;
using semestrovka1_oris.framework.core.Repositories;
using semestrovka1_oris.models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace semestrovka1_oris.framework.core.Controllers
{
    [HttpController("admin")]
    public class AdminController
    {
        OrmContext orm = WikiDatabase.Connect();
        public HtmlTemplateRenderer renderer = new HtmlTemplateRenderer();
        string layout = File.ReadAllText("static/layout.html");

        [HttpPOST]
        public async Task<(string, int, string)> DeleteRole(NameValueCollection query)
        {
            RolesRepository rep = new RolesRepository(orm);
            if (!int.TryParse(query["id"], out int id)) return ("400", 400, "");
            await rep.DeleteAsync(id);
            return ("", 303, "/admin/adminpanel");
        }
        [HttpPOST]
        public async Task<(string, int, string)> InsertRole(NameValueCollection query, UploadedFile? image)
        {
            if (!int.TryParse(query["turnOrder"], out int turnOrder) || turnOrder < 1)
                return ("Очерёдность хода должна быть положительным целым числом", 400, "");

            if (image == null)
                return ("Изображение не выбрано", 400, "");

            string imageUrl = await SaveImageAsync(image, "roles");

            RolesRepository rep = new RolesRepository(orm);

            await rep.InsertAsync(new Role
            {
                TurnOrder = turnOrder,
                Name = query["name"] ?? "",
                Description = query["description"] ?? "",
                ImageURL = imageUrl
            });

            return ("", 303, "/admin/adminpanel");
        }
        [HttpPOST]
        public async Task<(string, int, string)> UpdateRole(NameValueCollection query, UploadedFile? image)
        {
            RolesRepository rep = new RolesRepository(orm);
            if (!int.TryParse(query["id"], out int id)) { return ("400", 400, ""); }
            if (!int.TryParse(query["turnOrder"], out int turnOrder) || turnOrder < 1)
                return ("Очерёдность хода должна быть положительным целым числом", 400, "");

            var roles = await rep.GetAllAsync();
            var oldRole = roles.FirstOrDefault(r => r.Id == id);
            if (oldRole == null)
                return ("Персонаж не найден", 404, "");

            string imageUrl = oldRole.ImageURL;
            string name = query["name"] ?? "";
            string description = query["description"] ?? "";

            if (image != null && image.Data.Length > 0)
            {
                imageUrl = await SaveImageAsync(image, "roles");
            }

            
            await rep.UpdateAsync(id, new Role
            {
                TurnOrder = turnOrder,
                Description = description,
                Name = name,
                ImageURL = imageUrl
            });
            return ("", 303, "/admin/adminpanel");
        }
        [HttpGET]
        public async Task<(string, int, string)> AdminPanel(NameValueCollection query)
        {
            string template = File.ReadAllText("static/templates/admin_panel.html");
            RolesRepository rolesRep = new RolesRepository(orm);
            DistrictsRepository distrRep = new DistrictsRepository(orm);
            var roles = await rolesRep.GetAllAsync();
            var districts = await distrRep.GetAllAsync();
            var dataModel = new { roles, districts };
            string contentHtml = renderer.RenderFromString4(template, dataModel);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200, "");
        }
        [HttpGET]
        public async Task<(string, int, string)> EditRole(NameValueCollection query)
        {
            if (!int.TryParse(query["id"], out int id)) return ("400", 400, "");
            RolesRepository rep = new RolesRepository(orm);
            var roles = await rep.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return ("404", 404, "");

            string template = File.ReadAllText("static/templates/edit_role.html");
            string contentHtml = renderer.RenderFromString(template, role);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200, "");
        }
        [HttpGET]
        public async Task<(string, int, string)> AddRole(NameValueCollection query)
        {
            string contentHtml = File.ReadAllText("static/templates/add_role.html");
            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200, "");
        }

        [HttpPOST]
        public async Task<(string, int, string)> UpdateDistrict(NameValueCollection query, UploadedFile? image)
        {
            DistrictsRepository rep = new DistrictsRepository(orm);
            if (!int.TryParse(query["id"], out int id)) { return ("400", 400, ""); }
            var roles = await rep.GetAllAsync();
            var oldRole = roles.FirstOrDefault(r => r.Id == id);
            if (oldRole == null)
                return ("Персонаж не найден", 404, "");

            string imageUrl = oldRole.ImageURL;
            string name = query["name"] ?? "";
            string description = query["description"] ?? "";
            string color = query["color"] ?? "";
            if (image != null && image.Data.Length > 0)
            {
                imageUrl = await SaveImageAsync(image, "districts");
            }

            if (!int.TryParse(query["cost"], out int cost)) return ("400", 400, "");
            if (!int.TryParse(query["quantity"], out int quantity)) return ("400", 400, "");

            await rep.UpdateAsync(id, new District() { Description = description, Name = name, ImageURL = imageUrl, Color = color, Cost = cost, Quantity = quantity });
            return ("", 303, "/admin/adminpanel");
        }
        [HttpPOST]
        public async Task<(string, int, string)> InsertDistrict(NameValueCollection query, UploadedFile? image)
        {
            DistrictsRepository rep = new DistrictsRepository(orm);
            if (image == null)
                return ("Изображение не выбрано", 400, "");

            string imageUrl = await SaveImageAsync(image, "districts");
            string name = query["name"] ?? "";
            string description = query["description"] ?? "";
            string color = query["color"] ?? "";

            if (!int.TryParse(query["cost"], out int cost))
            {
                Console.WriteLine($"Cost: {query["cost"]}");
                return ("400", 400, "");
            }
            if (!int.TryParse(query["quantity"], out int quantity))
            {
                Console.WriteLine($"Quantity: {query["quantity"]}");
                return ("400", 400, "");
            }


            await rep.InsertAsync(new District() { Description = description, Name = name, ImageURL = imageUrl, Color = color, Cost = cost, Quantity = quantity });
            return ("", 303, "/admin/adminpanel");
        }
        [HttpGET]
        public async Task<(string, int, string)> EditDistrict(NameValueCollection query)
        {
            if (!int.TryParse(query["id"], out int id)) return ("400", 400, "");
            DistrictsRepository rep = new DistrictsRepository(orm);
            var roles = await rep.GetAllAsync();
            var role = roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return ("404", 404, "");

            string template = File.ReadAllText("static/templates/edit_district.html");
            string contentHtml = renderer.RenderFromString(template, role);

            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200, "");
        }
        [HttpGET]
        public async Task<(string, int, string)> AddDistrict(NameValueCollection query)
        {
            string contentHtml = File.ReadAllText("static/templates/add_district.html");
            string result = layout.Replace("{{Content}}", contentHtml);
            return (result, 200, "");
        }
        [HttpPOST]
        public async Task<string> SaveImageAsync(UploadedFile image, string folder)
        {
            if (image == null || image.Data.Length == 0)
                throw new InvalidDataException();


            byte[] data = image.Data;

            string? extension = null;

            Console.WriteLine("=== IMAGE DEBUG ===");
            Console.WriteLine($"Имя: {image.FileName}");
            Console.WriteLine($"Размер: {image.Data.Length} байт");
            Console.WriteLine(
                $"Первые байты: {Convert.ToHexString(image.Data.Take(12).ToArray())}"
            );

            if (data.Length >= 8 &&
                data.AsSpan(0, 8).SequenceEqual(
                    new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }))
            {
                extension = ".png";
            }
            else if (data.Length >= 3 &&
                     data[0] == 0xFF &&
                     data[1] == 0xD8 &&
                     data[2] == 0xFF)
            {
                extension = ".jpg";
            }
            else if (data.Length >= 12 &&
                     Encoding.ASCII.GetString(data, 0, 4) == "RIFF" &&
                     Encoding.ASCII.GetString(data, 8, 4) == "WEBP")
            {
                extension = ".webp";
            }

            if (extension == null)
                throw new InvalidDataException();


            string fileName = Guid.NewGuid().ToString("N") + extension;

            string directory = Path.Combine(
                Directory.GetCurrentDirectory(),
                "static",
                "images",
                folder
            );

            Directory.CreateDirectory(directory);

            await File.WriteAllBytesAsync(
                Path.Combine(directory, fileName),
                data
            );

            return $"/images/{folder}/" + fileName;
        }
    }
}
