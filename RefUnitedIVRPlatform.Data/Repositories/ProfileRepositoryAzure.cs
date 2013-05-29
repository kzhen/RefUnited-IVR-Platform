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

    public ProfileRepositoryAzure(string connectionString)
    {
      this.storageAccount = CloudStorageAccount.Parse(connectionString);
      this.tableClient = storageAccount.CreateCloudTableClient();
      CheckAndCreateStorage();
    }

    private void CheckAndCreateStorage()
    {
      this.profilesTable = tableClient.GetTableReference("ivrProfile");
      profilesTable.CreateIfNotExists();
    }

    public IVRProfile GetByPhoneNumber(string lookupPhoneNumber)
    {
      throw new NotImplementedException();
    }

    public IVRProfile Get(int id)
    {
      throw new NotImplementedException();
    }

    public IVRProfile Create(IVRProfile item)
    {
      var entity = IVRProfileToEntityMapper.ConvertToEntity(item);

      TableOperation insertOperation = TableOperation.Insert(entity);
      var result = profilesTable.Execute(insertOperation);

      throw new NotImplementedException();
    }

    public bool Delete(IVRProfile item)
    {
      throw new NotImplementedException();
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
