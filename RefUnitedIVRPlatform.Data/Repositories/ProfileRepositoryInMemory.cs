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
    private readonly List<IVRProfile> profiles;

    public ProfileRepositoryInMemory()
    {
      profiles = new List<IVRProfile>();
    }

    private bool Exists(IVRProfile profile)
    {
      return profiles.Exists(m => m.ProfileId == profile.ProfileId);
    }

    public IVRProfile GetByPhoneNumber(string lookupPhoneNumber)
    {
      return profiles.SingleOrDefault(m => m.PhoneNumber.Equals(lookupPhoneNumber, StringComparison.OrdinalIgnoreCase));
    }

    public IVRProfile Get(int profileId)
    {
      return profiles.SingleOrDefault(m => m.ProfileId == profileId);
    }

    public IVRProfile Create(IVRProfile item)
    {
      if (item == null)
      {
        throw new ArgumentNullException("item");
      }
      if (item.ProfileId <= 0)
      {
        throw new ArgumentException("profileId must be set");
      }

      if (Exists(item))
      {
        return Update(item);
      }

      profiles.Add(item);

      return item;
    }

    public bool Delete(IVRProfile item)
    {
      throw new NotImplementedException();
    }

    public List<IVRProfile> GetAll()
    {
      return profiles;
    }

    public IVRProfile Update(IVRProfile item)
    {
      if (item.ProfileId <= 0)
      {
        throw new ArgumentException("profileId must be set");
      }

      if (!Exists(item))
      {
        return Create(item);
      }

      var inListProfile = profiles.Single(m => m.ProfileId == item.ProfileId);

      profiles.Remove(inListProfile);
      profiles.Add(item);

      return item;
      
    }
  }
}
