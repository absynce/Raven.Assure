using System.Collections.Generic;
using System.IO.Abstractions;
using Raven.Abstractions.Data;
using Raven.Assure.ResourceDocument;
using Xunit;

namespace Raven.Assure.Test.ResourceDocument
{
   public class DatabaseDocumentServiceTests
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
                  var resourceDocumentService = new DatabaseDocumentService(new FileSystem());

                  var encryptedDatabaseDocument = new DatabaseDocument()
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
                  var resourceDocumentService = new DatabaseDocumentService(new FileSystem());

                  var originalDatabaseDocument = new DatabaseDocument()
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