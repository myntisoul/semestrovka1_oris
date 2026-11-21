using HttpMultipartParser;
using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;

namespace semestrovka1_oris.framework.core;

public class HttpServer
{
    private string filePath = Path.Combine("static", "index.html");
    private HttpListener _listener;

    private const long MaxRequestSize = 6 * 1024 * 1024;
    private const int MaxImageSize = 5 * 1024 * 1024;

    public void Start()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Ошибка: файл {filePath} не найден!");
            return;
        }

        _listener = new HttpListener();
        _listener.Prefixes.Add(
            Environment.GetEnvironmentVariable("ORIS_HTTP_PREFIX") ?? "http://127.0.0.1:8888/"
        );

        _listener.Start();
        Console.WriteLine("Сервер начал свою работу");

        Receive();
    }

    public void Stop()
    {
        _listener.Stop();
        Console.WriteLine("Сервер завершил свою работу");
    }

    private void Receive()
    {
        if (_listener.IsListening)
        {
            _listener.BeginGetContext(
                ListenerCallback,
                null
            );
        }
    }

    private async void ListenerCallback(IAsyncResult result)
    {
        HttpListenerContext? context = null;

        try
        {
            if (!_listener.IsListening)
                return;

            context = _listener.EndGetContext(result);

            var request = context.Request;
            var response = context.Response;

            string path = request.Url!.LocalPath;

            if (string.IsNullOrEmpty(path) ||
                path.EndsWith("/"))
            {
                path += "index.html";
            }

            string httpMethod = request.HttpMethod;
            NameValueCollection queryString = request.QueryString;

            FormData formData;

            bool isMultipart = request.ContentType?
                .StartsWith(
                    "multipart/form-data",
                    StringComparison.OrdinalIgnoreCase
                ) == true;

            string body = "";

            if (isMultipart)
            {
                if (request.ContentLength64 < 0 ||
                    request.ContentLength64 > MaxRequestSize)
                {
                    response.StatusCode = 413;
                    response.Close();
                    return;
                }

                try
                {
                    formData = await ParseMultipartAsync(request.InputStream);
                }
                catch (ArgumentException)
                {
                    response.StatusCode = 400;
                    response.Close();
                    return;
                }
                catch (InvalidDataException)
                {
                    response.StatusCode = 400;
                    response.Close();
                    return;
                }
            }
            else
            {
                if (request.ContentLength64 > MaxRequestSize)
                {
                    response.StatusCode = 413;
                    response.Close();
                    return;
                }
                using var reader = new StreamReader(
                    request.InputStream,
                    request.ContentEncoding
                );

                body = await reader.ReadToEndAsync();

                formData = new FormData
                {
                    Fields = ParseUrlEncoded(body)
                };
            }

            var (
                isHandledByController,
                responseHtml,
                statusCode,
                redirect
            ) = await TryRouteCore(
                httpMethod,
                path,
                body,
                queryString,
                formData
            );

            if (isHandledByController)
            {
                response.StatusCode = statusCode;

                if (!string.IsNullOrEmpty(redirect))
                {
                    response.RedirectLocation = redirect;
                }

                if (!string.IsNullOrEmpty(responseHtml))
                {
                    byte[] buffer =
                        Encoding.UTF8.GetBytes(responseHtml);

                    response.ContentLength64 = buffer.Length;
                    response.ContentType =
                        "text/html; charset=utf-8";

                    await response.OutputStream.WriteAsync(buffer);
                }
                else
                {
                    response.ContentLength64 = 0;
                }

                response.Close();
                return;
            }

            string staticRoot = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(), "static"));
            string staticFilePath = Path.GetFullPath(Path.Combine(
                staticRoot, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar)));
            if (!staticFilePath.StartsWith(staticRoot + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase))
            {
                response.StatusCode = 403;
                response.Close();
                return;
            }

            FileInfo fileInfo = new FileInfo(staticFilePath);

            if (!fileInfo.Exists)
            {
                response.StatusCode = 404;

                staticFilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "static",
                    "404.html"
                );

                fileInfo = new FileInfo(staticFilePath);
            }

            if (fileInfo.Exists)
            {
                switch (fileInfo.Extension.ToLowerInvariant())
                {
                    case ".html":
                        response.ContentType = "text/html; charset=utf-8";
                        break;

                    case ".css":
                        response.ContentType = "text/css; charset=utf-8";
                        break;

                    case ".js":
                        response.ContentType = "text/javascript; charset=utf-8";
                        break;

                    case ".png":
                        response.ContentType = "image/png";
                        break;

                    case ".jpg":
                    case ".jpeg":
                        response.ContentType = "image/jpeg";
                        break;

                    case ".webp":
                        response.ContentType = "image/webp";
                        break;

                    case ".svg":
                        response.ContentType = "image/svg+xml";
                        break;

                    case ".ico":
                        response.ContentType = "image/x-icon";
                        break;
                }

                byte[] buffer =
                    await File.ReadAllBytesAsync(staticFilePath);

                response.ContentLength64 = buffer.Length;

                await response.OutputStream.WriteAsync(buffer);
            }

            response.Close();
        }
        catch (HttpListenerException)
        {
            Console.WriteLine("Сервер остановлен");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex);

            try
            {
                if (context != null)
                {
                    context.Response.StatusCode = 500;
                    context.Response.Close();
                }
            }
            catch { }
        }
        finally
        {
            if (_listener.IsListening)
                Receive();
        }
    }

    private static NameValueCollection ParseUrlEncoded(
        string body)
    {
        var form = new NameValueCollection();

        foreach (string pair in body.Split('&',
                     StringSplitOptions.RemoveEmptyEntries))
        {
            string[] kv = pair.Split('=', 2);

            string key = WebUtility.UrlDecode(kv[0]);
            string value = kv.Length > 1
                ? WebUtility.UrlDecode(kv[1])
                : "";

            form.Add(key, value);
        }

        return form;
    }
    internal static async Task<FormData> ParseMultipartAsync(Stream body)
    {
        var parser = await MultipartFormDataParser.ParseAsync(
            body
        );

        var result = new FormData();

        foreach (var parameter in parser.Parameters)
        {
            result.Fields.Add(
                parameter.Name,
                parameter.Data
            );
        }

        var image = parser.Files.FirstOrDefault(
    f => f.Name == "image"
);

        if (image != null)
        {
            using var memory = new MemoryStream();

            if (image.Data.CanSeek)
            {
                image.Data.Position = 0;
            }

            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead = await image.Data.ReadAsync(buffer)) > 0)
            {
                if (memory.Length + bytesRead > MaxImageSize)
                {
                    throw new InvalidDataException(
                        "Изображение слишком большое"
                    );
                }

                await memory.WriteAsync(buffer.AsMemory(0, bytesRead));
            }

            result.Image = new UploadedFile
            {
                FileName = image.FileName,
                Data = memory.ToArray()
            };
        }

        if (image != null)
        {
            // Some parser versions leave the file stream at its end.
            if (image.Data.CanSeek)
                image.Data.Position = 0;

            using var memory = new MemoryStream();

            byte[] buffer = new byte[8192];
            int bytesRead;

            while ((bytesRead =
                    await image.Data.ReadAsync(buffer)) > 0)
            {
                if (memory.Length + bytesRead > MaxImageSize)
                {
                    throw new InvalidDataException(
                        "Изображение слишком большое"
                    );
                }

                await memory.WriteAsync(
                    buffer.AsMemory(0, bytesRead)
                );
            }

            result.Image = new UploadedFile
            {
                FileName = image.FileName,
                Data = memory.ToArray()
            };
        }

        return result;
    }

    public Task<(
        bool isHandled,
        string responseHtml,
        int code,
        string redirectionLocation
    )> TryRoute(
        string httpMethod,
        string path,
        string body,
        NameValueCollection queryString)
    {
        var formData = new FormData
        {
            Fields = ParseUrlEncoded(body)
        };

        return TryRouteCore(
            httpMethod,
            path,
            body,
            queryString,
            formData
        );
    }

    private async Task<(
        bool isHandled,
        string responseHtml,
        int code,
        string redirectionLocation
    )> TryRouteCore(
        string httpMethod,
        string path,
        string body,
        NameValueCollection queryString,
        FormData formData)
    {
        if (path.Contains(".") || path == "/")
            return (false, null, 200, null);

        string[] segments = path.Split(
            '/',
            StringSplitOptions.RemoveEmptyEntries
        );

        if (segments.Length == 0)
            return (false, null, 200, null);

        string controllerName = segments[0];
        string actionName = segments.Length > 1
            ? segments[1]
            : "index";

        var assembly = Assembly.GetExecutingAssembly();

        var controller = assembly.GetTypes()
            .Where(t => Attribute.IsDefined(
                t, typeof(HttpController)))
            .FirstOrDefault(c =>
            {
                var name = c.GetCustomAttribute<HttpController>()!.Name;

                return (
                    name ?? c.Name.Replace("Controller", "")
                ).Equals(
                    controllerName,
                    StringComparison.OrdinalIgnoreCase
                );
            });

        if (controller == null)
            return (false, null, 200, null);

        var method = controller.GetMethods()
            .Where(t => t.Name.Equals(
                actionName,
                StringComparison.OrdinalIgnoreCase
            ))
            .FirstOrDefault(t =>
                t.GetCustomAttributes(true)
                    .Any(attr => string.Equals(
                        attr.GetType().Name,
                        $"Http{httpMethod}",
                        StringComparison.OrdinalIgnoreCase
                    ))
            );

        if (method == null)
            return (false, null, 200, null);

        object?[] queryParams = method.GetParameters()
            .Select(p =>
            {
                if (p.ParameterType == typeof(NameValueCollection))
                {
                    return (object)(
                        httpMethod == "GET"
                            ? queryString
                            : formData.Fields
                    );
                }

                if (p.ParameterType == typeof(UploadedFile))
                {
                    return (object?)formData.Image;
                }

                if (p.ParameterType == typeof(FormData))
                {
                    return formData;
                }

                if (p.ParameterType == typeof(string) &&
                    p.Name == "body")
                {
                    return body;
                }

                return null;
            })
            .ToArray();

        var constructor = controller.GetConstructors().First();
        var ctorParams = constructor.GetParameters();

        object?[] ctorArgs = new object?[ctorParams.Length];

        var controllerInstance = Activator.CreateInstance(
            controller,
            ctorArgs
        );

        var ret = method.Invoke(controllerInstance, queryParams);

        if (ret is Task task)
        {
            await task;

            var resultProp = task.GetType().GetProperty("Result");

            ret = resultProp?.GetValue(task);
        }

        if (ret is ValueTuple<string, int, string> redirectTuple)
        {
            return (
                true,
                redirectTuple.Item1,
                redirectTuple.Item2,
                redirectTuple.Item3
            );
        }

        if (ret is ValueTuple<string, int> resultTuple)
        {
            return (
                true,
                resultTuple.Item1,
                resultTuple.Item2,
                null
            );
        }

        string responseHtml = ret?.ToString();

        return (true, responseHtml, 200, null);
    }
}
