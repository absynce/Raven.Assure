using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Raven.Abstractions.Data;
using Raven.Assure.ResourceDocument;
using Raven.Json.Linq;
using Xunit;
using Raven.Abstractions.Extensions;

namespace Raven.Assure.Test.ResourceDocument
{
   /// <summary>
   /// Used to test the base implementations in ResourceDocumentService.
   /// </summary>
   public class TestResourceDocumentService<TResourceDocument> : ResourceDocumentService<TResourceDocument>
   {
      public TestResourceDocumentService(IFileSystem fileSystem) : base(fileSystem)
      {
      }

      public override ResourceDocumentUpdate<TResourceDocument> TryRemoveEncryptionKey(TResourceDocument document)
      {
         throw new System.NotImplementedException();
      }
   }

   public class ResourceDocumentServiceTests
   {
      public class FindLatest
      {
         public class WhenGivenBaseDirectoryWithDatabaseDocumentAndNoIncrementalFolders
         {
            [Fact]
            public void ShouldReturnDatabaseDocument()
            {
               const string baseBackupLocation = @"C:\raven\backups\dorn";

               var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
               {
                  {
                     $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
                        @"
   {
     ""Id"": ""Dorn"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Dorn"",
       ""Raven/StorageTypeName"": ""Esent""
     },
     ""SecuredSettings"": {
       ""Raven/Encryption/Key"": null,
       ""Raven/Encryption/Algorithm"": ""System.Security.Cryptography.RijndaelManaged, mscorlib"",
       ""Raven/Encryption/KeyBitsPreference"": ""256"",
       ""Raven/Encryption/EncryptIndexes"": ""True""
     },
     ""Disabled"": false
   }"
                     )
                  },
                  { $"{baseBackupLocation}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
               });

               var resourceDocumentService = new TestResourceDocumentService<DatabaseDocument>(fileSystem);

               var actualDocument =
                  resourceDocumentService.FindLatest(baseBackupLocation);

               Assert.NotNull(actualDocument);
               Assert.Equal("Dorn", actualDocument.Id);
            }
         }

         public class WhenTheresAnIncrementalBackupSubFolder
         {
            [Fact]
            public void ShouldReturnDocumentFromLatestIncrementalFolder()
            {
               const string baseBackupLocation = @"C:\raven\backups\winterfell.bak";
               const string incrementalSubFolder = "Inc 1459-11-30 23-14-21";
               var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
               {
                  { $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
   @"
   {
     ""Id"": ""Winterfell"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Winterfell"",
       ""Raven/StorageTypeName"": ""Esent""
     },
     ""SecuredSettings"": {
       ""Raven/Encryption/Key"": null,
       ""Raven/Encryption/Algorithm"": ""System.Security.Cryptography.RijndaelManaged, mscorlib"",
       ""Raven/Encryption/KeyBitsPreference"": ""256"",
       ""Raven/Encryption/EncryptIndexes"": ""True""
     },
     ""Disabled"": false
   }"
                     )},
                  { $"{baseBackupLocation}\\{incrementalSubFolder}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
   @"
   {
     ""Id"": ""WinterfellIsHere"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Winterfell"",
       ""Raven/StorageTypeName"": ""Esent""
     },
     ""SecuredSettings"": {
       ""Raven/Encryption/Key"": ""XQht064yDEPgHCpjPA/sSXWqy/2gD7z5XJ3PCLYfREQ="",
       ""Raven/Encryption/Algorithm"": ""System.Security.Cryptography.RijndaelManaged, mscorlib"",
       ""Raven/Encryption/KeyBitsPreference"": ""256"",
       ""Raven/Encryption/EncryptIndexes"": ""True""
     },
     ""Disabled"": false
   }"
                     )},
                  { $"{baseBackupLocation}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
               });

               var resourceDocumentService = new TestResourceDocumentService<DatabaseDocument>(fileSystem);

               var actualDocument = resourceDocumentService.FindLatest(baseBackupLocation);

               Assert.NotNull(actualDocument);
               Assert.Equal("WinterfellIsHere", actualDocument.Id);
            }
         }
      }

      public class Load
      {
         public class WhenGivenValidResourceDocumentPath
         {
            [Fact]
            public void ShouldLoadDocument()
            {
               const string baseBackupLocation = @"C:\raven\backups\westeros";

               var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
               {
                  {
                     $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
                        @"
   {
     ""Id"": ""Westeros"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Westeros"",
       ""Raven/StorageTypeName"": ""Esent""
     },
     ""SecuredSettings"": {
       ""Raven/Encryption/Key"": null,
       ""Raven/Encryption/Algorithm"": ""System.Security.Cryptography.RijndaelManaged, mscorlib"",
       ""Raven/Encryption/KeyBitsPreference"": ""256"",
       ""Raven/Encryption/EncryptIndexes"": ""True""
     },
     ""Disabled"": false
   }"
                     )
                  },
                  { $"{baseBackupLocation}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
               });

               var resourceDocumentService = new TestResourceDocumentService<DatabaseDocument>(fileSystem);

               var actualDocument =
                  resourceDocumentService.Load($@"{baseBackupLocation}\{Constants.DatabaseDocumentFilename}");

               Assert.NotNull(actualDocument);
               Assert.Equal("Westeros", actualDocument.Id);

            }

            public class WhenGivenInvalidResourceDocumentPath
            {
               [Fact]
               public void ShouldReturnNull()
               {
                  const string baseBackupLocation = @"C:\raven\backups\westeros";

                  var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                  {
                     {
                        $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
                           @"
   {
     ""Id"": ""Westeros"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Westeros"",
       ""Raven/StorageTypeName"": ""Esent""
     },
     ""SecuredSettings"": {
       ""Raven/Encryption/Key"": null,
       ""Raven/Encryption/Algorithm"": ""System.Security.Cryptography.RijndaelManaged, mscorlib"",
       ""Raven/Encryption/KeyBitsPreference"": ""256"",
       ""Raven/Encryption/EncryptIndexes"": ""True""
     },
     ""Disabled"": false
   }"
                        )
                     },
                     {
                        $"{baseBackupLocation}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 })
                     }
                  });

                  var resourceDocumentService = new TestResourceDocumentService<DatabaseDocument>(fileSystem);

                  var actualDocument =
                     resourceDocumentService.Load($@"{baseBackupLocation}\invalid.document");

                  Assert.Null(actualDocument);
               }
            }
         }
      }

      public class Save
      {
         public class WhenGivenAValidDocument
         {
            public class AndAValidDirectory
            {
               [Fact]
               public void ShouldOverwriteExistingDocument()
               {
                  const string baseBackupLocation = @"C:\raven\backups\westeros";
                  var baseDocumentPath = $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}";

                  var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
                  {
                     {
                        baseDocumentPath, new MockFileData(
                           @"
   {
     ""Id"": ""Westeros"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Westeros"",
       ""Raven/StorageTypeName"": ""Esent""
     },
     ""SecuredSettings"": {
       ""Raven/Encryption/Key"": null,
       ""Raven/Encryption/Algorithm"": ""System.Security.Cryptography.RijndaelManaged, mscorlib"",
       ""Raven/Encryption/KeyBitsPreference"": ""256"",
       ""Raven/Encryption/EncryptIndexes"": ""True""
     },
     ""Disabled"": false
   }"
                        )
                     },
                     {
                        $"{baseBackupLocation}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 })
                     }
                  });

                  var resourceDocumentService = new TestResourceDocumentService<DatabaseDocument>(fileSystem);

                  var newDocumentToSave = new DatabaseDocument()
                  {
                     Id = "Meereen"
                  };

                  resourceDocumentService.Save(newDocumentToSave, baseDocumentPath);

                  var databaseDocumentText = fileSystem.File.ReadAllText(baseDocumentPath);
                  var actualSavedDocument = RavenJObject.Parse(databaseDocumentText).JsonDeserialization<DatabaseDocument>();

                  Assert.Equal(newDocumentToSave.Id, actualSavedDocument.Id);
               }
            }
         }
      }
   }
}