using System.Collections.Generic;
using System.IO.Abstractions;
using Raven.Abstractions.Data;
using Raven.Abstractions.FileSystem;
using Raven.Assure.ResourceDocument;
using Xunit;

namespace Raven.Assure.Test.ResourceDocument
{
   public class FileSystemDocumentServiceTests
   {
      public class TryRemoveEncryptionKey
      {
         public class WhenGivenAValidDocument
         {
            public class ThatHasAnEncryptionKey
            {
               [Fact]
               public void ShouldRemoveEncryptionKey()
               {
                  var resourceDocumentService = new FileSystemDocumentService(new FileSystem());

                  var encryptedDatabaseDocument = new FileSystemDocument
                  {
                     Id = "WestOfWesteros",
                     SecuredSettings = new Dictionary<string, string>()
                     {
                        { "Raven/Encryption/Key", "asdf" },
                        { "Raven/Encryption/EncryptIndexes", "True" }
                     }
                  };

                  var documentUpdate = resourceDocumentService.TryRemoveEncryptionKey(encryptedDatabaseDocument);

                  Assert.True(documentUpdate.Updated, "It should have set the status to updated.");
                  Assert.Equal(encryptedDatabaseDocument.Id, documentUpdate.Document.Id);
                  Assert.Null(documentUpdate.Document.SecuredSettings["Raven/Encryption/Key"]);
               }
            }

            public class WithoutAnEncryptionKey
            {
               [Fact]
               public void ShouldReturnSameDocument()
               {
                  var resourceDocumentService = new FileSystemDocumentService(new FileSystem());

                  var originalDatabaseDocument = new FileSystemDocument
                  {
                     Id = "TheValeOfArryn"
                  };

                  var documentUpdate = resourceDocumentService.TryRemoveEncryptionKey(originalDatabaseDocument);

                  Assert.False(documentUpdate.Updated, "It should not have set the status to updated.");
                  Assert.Equal(originalDatabaseDocument, documentUpdate.Document);
               }
            }
         }
      }
   }
}