using Raven.Assure.Restore;
using Xunit;

namespace Raven.Assure.Test.Restore
{
   public class RestoreDatabaseTests
   {
      public class From
      {
         [Fact]
         public void ShouldSetBackupLocation()
         {
            var restorer = new RestoreDatabase();

            const string expectedBackupLocation = "C:\\temp\\good.will";

            var actualRestorer = restorer.From(expectedBackupLocation);

            Assert.Equal(expectedBackupLocation, actualRestorer.BackupLocation);
         }
      }

      public class To
      {
         [Fact]
         public void ShouldSetDatabaseName()
         {
            var restorer = new RestoreDatabase();

            const string expectedDatabaseName = "good.will";

            var actualRestorer = restorer.To(expectedDatabaseName);

            Assert.Equal(expectedDatabaseName, actualRestorer.DatabaseName);
         }
      }

      public class In
      {
         [Fact]
         public void ShouldSetRestoreLocation()
         {
            var restorer = new RestoreDatabase();

            const string expectedRestoreLocation = @"~\Databases\good.will";

            var actualRestorer = restorer.In(expectedRestoreLocation);

            Assert.Equal(expectedRestoreLocation, actualRestorer.RestoreLocation);
         }
      }

      public class At
      {
         [Fact]
         public void ShouldSetServerUrl()
         {
            var restorer = new RestoreDatabase();

            const string expectedServerUrl = "http://good-will-hunting.org/";

            var actualRestorer = restorer.At(expectedServerUrl);

            Assert.Equal(expectedServerUrl, actualRestorer.ServerUrl);
         }
      }

      public class Run
      {
         [Fact(Skip = "Manual testing only. Would need to pass store to allow testability.")]
         public void ShouldActuallyRestoreMyTestDb()
         {
            var restorer = new RestoreDatabase()
               .From(@"C:\temp\test2.bak")
               .To("test2")
               .At("http://localhost:8080")
               //.In(@"C:\RavenDB\Databases\test")
               .Run();
      
         }
      }
   }
}