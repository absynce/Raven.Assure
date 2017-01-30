using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using Raven.Assure.Fluent;
using Raven.Client;
using Raven.Client.Connection;
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
         [Fact]
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
            var filePath = @"C:\temp\test.qa.bak";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
               { $"{filePath}\\some.json", new MockFileData(
@"
{
  ""Id"": null,
  ""Settings"": {
    ""Raven/StorageTypeName"": ""Esent""
  },
  ""SecuredSettings"": {},
  ""Disabled"": false
}"
                  )},
               { $"{filePath}\\fileOne.export", new MockFileData(new byte[] { 0x12, 0x34, 0x56, 0xd2 }) }
            });

            var backUpper = new BackUp()
               .On(fileSystem);

            backUpper.removeEncryptionKey();

            Assert.True(false, "TODO: Get the document and see if it has the encryption key.");
         }
      }
   }
}
