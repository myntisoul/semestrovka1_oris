using Microsoft.VisualStudio.TestTools.UnitTesting;
using semestrovka1_oris.framework.core;

namespace Semestrovka.Tests;

[TestClass]
public class TemplateTests
{
    private readonly HtmlTemplateRenderer renderer = new();

    [TestMethod]
    public void ReplacesPropertiesAndKeepsUnknownPlaceholders()
    {
        const string template = "<h2>${role.Name}</h2><p>${role.TurnOrder} ${role.Unknown}</p>";
        var model = new { Name = "Маг", TurnOrder = 3 };

        Assert.AreEqual("<h2>Маг</h2><p>3 ${role.Unknown}</p>",
            renderer.RenderFromString(template, model));
    }

    [TestMethod]
    public void ReplacesNullWithEmptyText()
    {
        Assert.AreEqual("<p></p>", renderer.RenderFromString(
            "<p>${role.Name}</p>", new { Name = (string?)null }));
    }

    [TestMethod]
    public void ReplacesNestedProperty()
    {
        var model = new { Role = new { Name = "Маг" } };
        Assert.AreEqual("Маг", renderer.RenderFromString2("${ model.Role.Name}", model));
    }

    [DataTestMethod]
    [DataRow(true, "yes")]
    [DataRow(false, "no")]
    public void ChoosesConditionalBranch(bool visible, string expected)
    {
        Assert.AreEqual(expected, renderer.RenderFromString3(
            "$if (Visible)yes$elseno", new { Visible = visible }));
    }

    [TestMethod]
    public void RendersEachItemInCollectionOrder()
    {
        var roles = new[] { new { Name = "Вор", Id = 1000 }, new { Name = "Маг", Id = 3 } };
        const string template = "$foreach (Role role in roles){<a href=\"?id=role.Id\">role.Name</a>}";

        Assert.AreEqual("<a href=\"?id=1000\">Вор</a><a href=\"?id=3\">Маг</a>",
            renderer.RenderFromString4(template, new { roles }));
    }

    [TestMethod]
    public void RendersEmptyCollectionWithoutItems()
    {
        const string template = "<ul>$foreach (Role role in roles){<li>role.Name</li>}</ul>";
        Assert.AreEqual("<ul></ul>", renderer.RenderFromString4(template, new { roles = Array.Empty<object>() }));
    }
}
