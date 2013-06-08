using RefUnitedIVRPlatform.Common.Entities;
using RefUnitedIVRPlatform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using RefUnitedIVRPlatform.Data.DtoConverter;
using RefUnitedIVRPlatform.Data.DtoEntities;


namespace RefUnitedIVRPlatform.Data.Repositories
{
  public class ProfileRepositoryAzure : IProfileRepository
  {
    private CloudStorageAccount storageAccount;
    private CloudTableClient tableClient;
    private CloudTable profilesTable;

    private const string AZURE_IVRPROFILE_TABLE = "ivrProfile";

    public ProfileRepositoryAzure(string connectionString)
    {
      if (connectionString.Equals("DEVELOPMENT_CONNECTION"))
      {
        this.storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
      }
      else
      {
        this.storageAccount = CloudStorageAccount.Parse(connectionString);
      }
      
      this.tableClient = storageAccount.CreateCloudTableClient();
      CheckAndCreateStorage();
    }

    private void CheckAndCreateStorage()
    {
      this.profilesTable = tableClient.GetTableReference(AZURE_IVRPROFILE_TABLE);
      profilesTable.CreateIfNotExists();
    }

    public IVRProfile GetByPhoneNumber(string lookupPhoneNumber)
    {
      TableQuery<IVRProfileEntity> query = new TableQuery<IVRProfileEntity>().Where(
        TableQuery.GenerateFilterCondition("PhoneNumber", QueryComparisons.Equal, lookupPhoneNumber));

      var results = profilesTable.ExecuteQuery<IVRProfileEntity>(query).First();

      var profile = IVRProfileToEntityMapper.ConvertFromEntity(results);

      return profile;
    }

    public IVRProfile Get(int id)
    {
      string partitionKey = id.ToString().Substring(0, 2);
      string rowKey = id.ToString();

      TableOperation getOperation = TableOperation.Retrieve<IVRProfileEntity>(partitionKey, rowKey);

      var result = profilesTable.Execute(getOperation);

      var entity = (IVRProfileEntity)result.Result;

      var returnValue = IVRProfileToEntityMapper.ConvertFromEntity(entity);

      return returnValue;
    }

    public IVRProfile Create(IVRProfile item)
    {
      var entity = IVRProfileToEntityMapper.ConvertToEntity(item);

      TableOperation insertOperation = TableOperation.Insert(entity);
      var result = profilesTable.Execute(insertOperation);


      IVRProfileEntity entityResult = (IVRProfileEntity)result.Result;

      return IVRProfileToEntityMapper.ConvertFromEntity(entityResult);
    }


    public bool Delete(IVRProfile item)
    {
      //Azure storage will throw a StorageException --- 404 --- if it cannot find the entity to delete.

      var entity = IVRProfileToEntityMapper.ConvertToEntity(item);

      entity.ETag = "*";

      TableOperation deleteOperation = TableOperation.Delete(entity);

      try
      {
        var result = profilesTable.Execute(deleteOperation);

        return true;
      }
      catch (StorageException)
      {
        return false;
      }
    }

    public List<IVRProfile> GetAll()
    {
      throw new NotImplementedException();
    }

    public IVRProfile Update(IVRProfile profile)
    {
      throw new NotImplementedException();
    }
  }
}
