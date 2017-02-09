using Raven.Abstractions.Data;
using Raven.Abstractions.FileSystem;

namespace Raven.Assure.ResourceDocument
{
   public class ResourceDocumentUpdate<T>
   {
      public T Document { get; set; }
      public bool Updated { get; set; }
   }
}