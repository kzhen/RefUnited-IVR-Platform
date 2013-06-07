using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace RefUnitedIVRPlatform.Data.Tests
{
  [TestClass]
  public class AzureStorageTests
  {
    private class TestEntity : TableEntity
    {
      public string ID { get; set; }
      public string FirstName { get; set; }
      public string LastName { get; set; }

      public TestEntity()
      {
        this.RowKey = ID;
        this.PartitionKey = LastName;
      }
    }

    private CloudStorageAccount storageAccount;
    private CloudTableClient tableClient;
    private CloudTable table1;

    [TestInitialize]
    public void Setup()
    {
      this.storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
      this.tableClient = storageAccount.CreateCloudTableClient();
      this.table1 = tableClient.GetTableReference("table1");
      this.table1.DeleteIfExists();
      this.table1.Create();

      var entityToCreate = new TestEntity()
      {
        ID = "1",
        FirstName = "Cecil",
        LastName = "Cedar"
      };

      entityToCreate.PartitionKey = entityToCreate.LastName;
      entityToCreate.RowKey = entityToCreate.ID;
    }

    [TestMethod]
    public void ShouldCreateAnEntity()
    {
      var entityToCreate = new TestEntity()
      {
        ID = "2",
        FirstName = "Bob",
        LastName = "Brown"
      };

      entityToCreate.PartitionKey = entityToCreate.LastName;
      entityToCreate.RowKey = entityToCreate.ID;

      TableOperation insertOperation = TableOperation.Insert(entityToCreate);

      var result = table1.Execute(insertOperation);

      TestEntity entityResult = (TestEntity)result.Result;

      Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ShouldReturnAResult()
    {
      TableOperation retrievalOperation = TableOperation.Retrieve<TestEntity>("Cedar", "1");
      TableResult result = table1.Execute(retrievalOperation);

      Assert.IsNotNull(result.Result);

      var entity = (TestEntity)result.Result;

      Assert.IsNotNull(entity);
    }

    [TestMethod]
    public void ShouldDeleteAnEntity()
    {
      var entityToCreate = new TestEntity()
      {
        ID = "3",
        FirstName = "Bob1",
        LastName = "Brown1"
      };

      entityToCreate.PartitionKey = entityToCreate.LastName;
      entityToCreate.RowKey = entityToCreate.ID;

      TableOperation insertOperation = TableOperation.Insert(entityToCreate);

      var result = table1.Execute(insertOperation);

      TestEntity entityResult = (TestEntity)result.Result;

      TableOperation deleteOperation = TableOperation.Delete(entityToCreate);

      var deletResult = table1.Execute(deleteOperation);

      entityToCreate.PartitionKey = "1234";
      entityToCreate.RowKey = "9999";
      TableOperation deleteOperation2 = TableOperation.Delete(entityToCreate);

      var deletResult2 = table1.Execute(deleteOperation2);
    }
  }
}
