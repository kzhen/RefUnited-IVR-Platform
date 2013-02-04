using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefUnitedIVRPlatform.Common.Interfaces;
using RefUnitedIVRPlatform.Common.Entities;

namespace RefUnitedIVRPlatform.Data.Repositories
{
  public class ProfileRepositoryInMemory : IProfileRepository
  {

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

    public void Update(IVRProfile profile)
    {
      throw new NotImplementedException();
    }
  }
}
