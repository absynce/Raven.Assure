using System.Collections.Generic;

namespace Raven.Assure.ResourceDocument
{
   public interface IResourceDocument
   {
      bool Disabled { get; set; }
      string Id { get; set; }
      Dictionary<string, string> SecuredSettings { get; set; }
      Dictionary<string, string> Settings { get; set; }
   }
}