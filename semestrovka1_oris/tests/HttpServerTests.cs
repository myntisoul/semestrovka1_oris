using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using semestrovka1_oris.framework.core;

namespace Semestrovka.Tests;

[TestClass]
public class HttpServerTests
{
    private readonly HttpServer server = new();

    [DataTestMethod]
    [DataRow("GET", "/")]
    [DataRow("GET", "/search.js")]
    [DataRow("GET", "/unknown/index")]
    [DataRow("GET", "/wiki/unknown")]
    [DataRow("POST", "/wiki/rules")]
    public async Task LeavesUnmatchedRequestsForStaticHandling(string method, string path)
    {
        var result = await server.TryRoute(method, path, "", new NameValueCollection());

        Assert.IsFalse(result.isHandled);
    }

    [TestMethod]
    public async Task RendersRulesInsideLayout()
    {
        var result = await server.TryRoute("GET", "/WiKi/RuLeS", "", new NameValueCollection());

        Assert.IsTrue(result.isHandled);
        Assert.AreEqual(200, result.code);
        StringAssert.Contains(result.responseHtml, "Цитадели Wiki");
        StringAssert.Contains(result.responseHtml, "Владелец короны");
        Assert.IsFalse(result.responseHtml.Contains("{{Content}}"));
    }

    [TestMethod]
    public async Task PassesQueryParametersToAsyncGetAction()
    {
        var query = new NameValueCollection { ["id"] = "invalid" };
        var result = await server.TryRoute("GET", "/wiki/rolesdetails", "id=1", query);

        Assert.IsTrue(result.isHandled);
        Assert.AreEqual(400, result.code);
    }

    [DataTestMethod]
    [DataRow("turnOrder=%30", "Очерёдность")]
    [DataRow("turnOrder=%31", "Изображение не выбрано")]
    public async Task DecodesPostBodyAndPassesItToAsyncAction(string body, string message)
    {
        var query = new NameValueCollection { ["turnOrder"] = "-1" };
        var result = await server.TryRoute("POST", "/admin/insertrole", body, query);

        Assert.IsTrue(result.isHandled);
        Assert.AreEqual(400, result.code);
        StringAssert.Contains(result.responseHtml, message);
    }

    [TestMethod]
    public async Task ParsesMultipartFieldsAndPreservesUploadedBytes()
    {
        using var form = new MultipartFormDataContent("OrisTestBoundary");
        form.Add(new StringContent("3"), "turnOrder");
        byte[] image = [137, 80, 78, 71, 13, 10, 26, 10, 0, 255];
        form.Add(new ByteArrayContent(image), "image", "card.png");
        await using var body = await form.ReadAsStreamAsync();

        var result = await HttpServer.ParseMultipartAsync(body);

        Assert.AreEqual("3", result.Fields["turnOrder"]);
        Assert.IsNotNull(result.Image);
        Assert.AreEqual("card.png", result.Image.FileName);
        CollectionAssert.AreEqual(image, result.Image.Data);
    }
}
