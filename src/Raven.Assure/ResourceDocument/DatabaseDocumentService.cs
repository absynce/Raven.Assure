using System.IO.Abstractions;
using Raven.Abstractions.Data;

namespace Raven.Assure.ResourceDocument
{
   public class DatabaseDocumentService : ResourceDocumentService<DatabaseDocument>
   {
      public DatabaseDocumentService(IFileSystem fileSystem) : base(fileSystem)
      {
      }

      public override ResourceDocumentUpdate<DatabaseDocument> TryRemoveEncryptionKey(DatabaseDocument databaseDocument)
      {
         if (!databaseDocument.SecuredSettings.ContainsKey(encryptionKeySettingKey))
         {
            return new ResourceDocumentUpdate<DatabaseDocument>
            {
               Document = databaseDocument,
               Updated = false
            };
         }

         // Remove encryption key
         databaseDocument.SecuredSettings[encryptionKeySettingKey] = null;

         return new ResourceDocumentUpdate<DatabaseDocument>
         {
            Document = databaseDocument,
            Updated = true
         };
      }

      protected override string getResourceDocumentFileName()
      {
         return Constants.DatabaseDocumentFilename;
      }
   }
}