using Raven.Assure.Fluent;
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
            var backup = new BackUp();

            const string databaseName = "sun.faces";
            const string backupLocation = "sun.faces.incremental.bak";
            const string serverUrl = "socal://garden-grove.ca";

            backup
               .From(databaseName)
               .At(serverUrl)
               .To(backupLocation);

            var actualResult = backup.Run();

            Assert.True(actualResult, "The backup should run.");
         }
      }
   }
}
