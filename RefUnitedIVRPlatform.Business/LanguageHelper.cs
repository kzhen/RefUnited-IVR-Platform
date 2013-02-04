using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RefUnitedIVRPlatform.Business
{
  internal class LanguageHelper
  {
    /// <summary>
    /// Param string - refers to the culture code e.g. en for English, es for Spanish etc...
    /// Param bool - if true then it means the resource file will need to supply a fully formed url to an MP3, if false then it is expected as text
    /// 
    /// The first will always be the 'default' culture
    /// </summary>
    private static readonly Dictionary<string, bool> cultures = new Dictionary<string, bool>()
    {
      {"en", false},
      {"es", false},
      {"ar", true}
    };

    public static string GetValidCulture(string name)
    {
      if (string.IsNullOrEmpty(name))
        return GetDefaultCulture(); // return Default culture

      if (cultures.ContainsKey(name))
        return name;

      // Find a close match. For example, if you have "en-US" defined and the user requests "en-GB", 
      // the function will return closes match that is "en-US" because at least the language is the same (ie English)            
      foreach (var c in cultures.Keys)
        if (c.StartsWith(name.Substring(0, 2)))
          return c;

      return GetDefaultCulture(); // return Default culture as no match found
    }

    public static string GetDefaultCulture()
    {
      return cultures.Keys.ElementAt(0); // return Default culture
    }

    public static bool IsImplementedAsMP3(string name)
    {
      return cultures[name];
    }
  }
}
