using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Assure.Fluent;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Json.Linq;
using Xunit;

namespace Raven.Assure.Test.Fluent
{
   public class BackUpTests
   {
      public class From
      {
         [Fact]
         public void ShouldSetDatabaseName()
         {
            var backup = new Raven.Assure.Fluent.BackUp();

            const string expectedDatabaseName = "sun.faces";
            var actualBackup = backup.From(expectedDatabaseName);

            Assert.Equal(actualBackup.DatabaseName, expectedDatabaseName);
         }
      }

      public class At
      {
         [Fact]
         public void ShouldSetServerUrl()
         {
            var backup = new Raven.Assure.Fluent.BackUp();

            const string expectedServerUrl = "db.sublime.com";
            var actualBackup = backup.At(expectedServerUrl);

            Assert.Equal(actualBackup.ServerUrl, expectedServerUrl);
         }
      }

      public class To
      {
         [Fact]
         public void ShouldSetToPath()
         {
            var backup = new BackUp();

            const string expectedToPath = "test.raven.incremental.bak";
            var actualBackup = backup.To(expectedToPath);

            Assert.Equal(expectedToPath, actualBackup.BackupLocation);
         }
      }

      public class Incremental
      {
         public class WhenPassedZeroParams
         {
            [Fact]
            public void ShouldSetIncrementalToTrue()
            {
               var backup = new BackUp();

               var actualBackup = backup.Incrementally();

               Assert.Equal(true, actualBackup.Incremental);
            }
         }
      }

      public class Run
      {
         [Fact(Skip = "Don't know how to test this yet.")]
         public void ShouldCallStartBackupWithSetOptions()
         {
            var mockDocumentStore = new Mock<IDocumentStore>();
            var mockGlobalDbCommands = new Mock<IGlobalAdminDatabaseCommands>();

            mockDocumentStore.Setup(store => store.Initialize());
            mockDocumentStore.Setup(store => store.DatabaseCommands.GlobalAdmin).Returns(mockGlobalDbCommands.Object);

            var backup = new BackUp();

            const string databaseName = "sun.faces";
            const string backupLocation = "sun.faces.incremental.bak";
            const string serverUrl = "socal://garden-grove.ca";

            backup
               //.UsingDocumentStore(mockDocumentStore.Object)
               .From(databaseName)
               .At(serverUrl)
               .To(backupLocation);

            //var actualResult = backup.Run();

            //Assert.True(actualResult, "The backup should run.");
            Assert.True(false, "Not sure how to test this yet. The method of running backups requires in-method instantiation of the store.");
         }

         [Fact]
         public void ShouldActuallyBackUpMyTestDb()
         {
            var backup = new BackUp()
               .From("test")
               .At("http://localhost:8080/")
               .To(@"C:\temp\test2.bak")
               .Incrementally()
               .Run();
         }
      }

      public class removeEncryptionKey
      {
         [Fact]
         public void ShouldRemoveEncryptionKeyFromBaseDatabaseDocument()
         {
            const string baseBackupLocation = @"C:\temp\test.qa.bak";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
               { $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
@"
{
  ""Id"": ""Test.QA"",
  ""Settings"": {
    ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
    ""Raven/DataDir"": ""~/Test.QA"",
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

            var backUpper = new BackUp()
               .To(baseBackupLocation)
               .On(fileSystem);

            backUpper.TryRemoveEncryptionKey();

            var baseDatabaseDocumentText = fileSystem.File.ReadAllText($"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}");
            var baseDatabaseDocument = RavenJObject.Parse(baseDatabaseDocumentText).JsonDeserialization<DatabaseDocument>();
            //Assert.True(false, "TODO: Get the document and see if it has the encryption key.");
            string encryptionKey;

            baseDatabaseDocument.SecuredSettings.TryGetValue("Raven/Encryption/Key", out encryptionKey);
            Assert.Null(encryptionKey);
         }

         public class WhenTheresNoEncryptionKey
         {
            [Fact]
            public void ShouldNotThrowAnException()
            {
               const string baseBackupLocation = @"C:\temp\test.qa.bak";
               var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
               {
                  {
                     $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
                        @"
{
  ""Id"": ""Test.QA"",
  ""Settings"": {
    ""Raven/ActiveBundles"": ""DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
    ""Raven/DataDir"": ""~/Test.QA"",
    ""Raven/StorageTypeName"": ""Esent""
  },
  ""SecuredSettings"": { },
  ""Disabled"": false
}"
                     )
                  },
                  { $"{baseBackupLocation}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
               });

               var backUpper = new BackUp()
                  .To(baseBackupLocation)
                  .On(fileSystem);

               backUpper.TryRemoveEncryptionKey();

               var baseDatabaseDocumentText = fileSystem.File
                  .ReadAllText($"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}");

               var baseDatabaseDocument = RavenJObject
                  .Parse(baseDatabaseDocumentText)
                  .JsonDeserialization<DatabaseDocument>();

               Assert.False(baseDatabaseDocument.SecuredSettings.ContainsKey("Raven/Encryption/Key"), "It should not have an encryption key setting.");
            }
         }

         public class WhenTheresAnIncrementalBackupSubFolder
         {
            [Fact]
            public void ShouldRemoveEncryptionKeyFromIncrementalBackupDatabaseDocument()
            {
               const string baseBackupLocation = @"C:\temp\test.qa.bak";
               const string incrementalSubFolder = "Inc 2017-01-30 23-14-21";
               var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
               {
                  { $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
   @"
   {
     ""Id"": ""Test.QA"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Test.QA"",
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
     ""Id"": ""Test.QA"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Test.QA"",
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

               var backUpper = new BackUp()
                  .To(baseBackupLocation)
                  .On(fileSystem);

               backUpper.TryRemoveEncryptionKey();

               var incrementalDatabaseDocumentText = fileSystem.File.ReadAllText($"{baseBackupLocation}\\{incrementalSubFolder}\\{Constants.DatabaseDocumentFilename}");
               var inrementalDatabaseDocument = RavenJObject.Parse(incrementalDatabaseDocumentText).JsonDeserialization<DatabaseDocument>();
               string encryptionKey;

               inrementalDatabaseDocument.SecuredSettings.TryGetValue("Raven/Encryption/Key", out encryptionKey);
               Assert.Null(encryptionKey);
            }
         }

         public class WhenTheresMultipleIncrementalBackupSubFolders
         {
            [Fact]
            public void ShouldRemoveEncryptionKeyFromLatestIncrementalBackupDatabaseDocument()
            {
               const string baseBackupLocation = @"C:\temp\test.qa.bak";
               const string incrementalSubFolder1 = "Inc 2017-01-30 23-14-21";
               const string incrementalSubFolder2 = "Inc 2017-01-31 13-27-08";
               var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
               {
                  { $"{baseBackupLocation}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
   @"
   {
     ""Id"": ""Test.QA"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Test.QA"",
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
                  { $"{baseBackupLocation}\\{incrementalSubFolder1}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
   @"
   {
     ""Id"": ""Test.QA"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Test.QA"",
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
                  { $"{baseBackupLocation}\\{incrementalSubFolder2}\\{Constants.DatabaseDocumentFilename}", new MockFileData(
   @"
   {
     ""Id"": ""Test.QA"",
     ""Settings"": {
       ""Raven/ActiveBundles"": ""Encryption;DocumentExpiration;Replication;SqlReplication;PeriodicExport;ScriptedIndexResults"",
       ""Raven/DataDir"": ""~/Test.QA"",
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

               var backUpper = new BackUp()
                  .To(baseBackupLocation)
                  .On(fileSystem);

               backUpper.TryRemoveEncryptionKey();

               var incrementalDatabaseDocumentText = fileSystem.File.ReadAllText($"{baseBackupLocation}\\{incrementalSubFolder2}\\{Constants.DatabaseDocumentFilename}");
               var inrementalDatabaseDocument = RavenJObject.Parse(incrementalDatabaseDocumentText).JsonDeserialization<DatabaseDocument>();
               string encryptionKey;

               inrementalDatabaseDocument.SecuredSettings.TryGetValue("Raven/Encryption/Key", out encryptionKey);
               Assert.Null(encryptionKey);
            }
         }
      }
   }
}
