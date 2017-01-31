using Raven.Assure.Log;

namespace Raven.Assure
{
   public struct DatabaseDocumentUpdate
   {
      public Abstractions.Data.DatabaseDocument DatabaseDocument { get; set; }
      public bool Updated { get; set; }
   }
}