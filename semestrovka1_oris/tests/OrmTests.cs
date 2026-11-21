using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ORM;

namespace Semestrovka.Tests;

[TestClass]
public class OrmTests
{
    [TestMethod]
    public void RejectsEmptyConnectionString()
    {
        Assert.ThrowsException<ArgumentException>(() => new OrmContext(" "));
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("dbo.Roles")]
    [DataRow("Roles Name")]
    [DataRow("Roles]")]
    public async Task CrudRejectsInvalidTableNamesBeforeOpeningConnection(string table)
    {
        var orm = new OrmContext("unused");
        var row = new TestRow();

        await Assert.ThrowsExceptionAsync<ArgumentException>(() => orm.GetAllAsync<TestRow>(table));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => orm.InsertAsync(table, row));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => orm.UpdateAsync(table, 1, row));
        await Assert.ThrowsExceptionAsync<ArgumentException>(() => orm.DeleteAsync(table, 1));
    }

    [TestMethod]
    public async Task MapsColumnsByNameAndConvertsValuesAndNulls()
    {
        using var table = new DataTable();
        table.Columns.Add("turnorder", typeof(short));
        table.Columns.Add("NAME", typeof(string));
        table.Columns.Add("Unused", typeof(string));
        table.Columns.Add("Id", typeof(int));
        table.Rows.Add((short)9, "Маг", "ignored", 1000);
        table.Rows.Add((short)2, DBNull.Value, "ignored", 23);
        await using var reader = table.CreateDataReader();

        var rows = await OrmContext.ReadRowsAsync<TestRow>(reader);

        Assert.AreEqual(2, rows.Count);
        Assert.AreEqual(1000, rows[0].Id);
        Assert.AreEqual("Маг", rows[0].Name);
        Assert.AreEqual(9, rows[0].TurnOrder);
        Assert.IsNull(rows[1].Name);
    }

    [TestMethod]
    [TestCategory("SqlServer")]
    public async Task CrudPreservesIdentityAndRoundTripsValues()
    {
        string? connectionString = Environment.GetEnvironmentVariable("ORIS_TEST_SQL");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Assert.Inconclusive("Set ORIS_TEST_SQL to a connection string for a test database.");
            return;
        }

        string table = $"OrmTest_{Guid.NewGuid():N}";
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        await using var create = new SqlCommand(
            $"CREATE TABLE [{table}] (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(100) NULL, TurnOrder INT NOT NULL)",
            connection);
        await create.ExecuteNonQueryAsync();

        try
        {
            var orm = new OrmContext(connectionString);
            await orm.InsertAsync(table, new TestRow { Id = 1000, Name = "Карточка 'Вор'", TurnOrder = 2 });
            var inserted = (await orm.GetAllAsync<TestRow>(table)).Single();
            Assert.AreEqual(1, inserted.Id);
            Assert.AreEqual("Карточка 'Вор'", inserted.Name);
            Assert.AreEqual(2, inserted.TurnOrder);

            Assert.AreEqual(1, await orm.UpdateAsync(table, inserted.Id,
                new TestRow { Id = 1000, Name = null, TurnOrder = 9 }));
            var updated = (await orm.GetAllAsync<TestRow>(table)).Single();
            Assert.AreEqual(inserted.Id, updated.Id);
            Assert.IsNull(updated.Name);
            Assert.AreEqual(9, updated.TurnOrder);

            Assert.AreEqual(1, await orm.DeleteAsync(table, inserted.Id));
            Assert.AreEqual(0, await orm.DeleteAsync(table, inserted.Id));
            Assert.AreEqual(0, (await orm.GetAllAsync<TestRow>(table)).Count);
        }
        finally
        {
            await using var drop = new SqlCommand($"DROP TABLE [{table}]", connection);
            await drop.ExecuteNonQueryAsync();
        }
    }

    public sealed class TestRow
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TurnOrder { get; set; }
    }
}
