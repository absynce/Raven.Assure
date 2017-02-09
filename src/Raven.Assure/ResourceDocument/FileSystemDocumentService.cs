using System.IO.Abstractions;
using Raven.Abstractions.Data;
using Raven.Abstractions.FileSystem;

namespace Raven.Assure.ResourceDocument
{
   public class FileSystemDocumentService : ResourceDocumentService<FileSystemDocument>
   {
      public FileSystemDocumentService(IFileSystem fileSystem) : base(fileSystem)
      {
      }

      public override ResourceDocumentUpdate<FileSystemDocument> TryRemoveEncryptionKey(FileSystemDocument fileSystemDocument)
      {
         if (!fileSystemDocument.SecuredSettings.ContainsKey(encryptionKeySettingKey))
         {
            return new ResourceDocumentUpdate<FileSystemDocument>
            {
               Document = fileSystemDocument,
               Updated = false
            };
         }

         // Remove encryption key
         fileSystemDocument.SecuredSettings[encryptionKeySettingKey] = null;

         return new ResourceDocumentUpdate<FileSystemDocument>
         {
            Document = fileSystemDocument,
            Updated = true
         };
      }

      protected override string getResourceDocumentFileName()
      {
         return Constants.FilesystemDocumentFilename;
      }
   }
}