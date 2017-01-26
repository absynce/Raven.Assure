using Raven.Assure.Fluent;
using Xunit;

namespace Raven.Assure.Test.Fluent
{
   public class RestoreTests
   {
      public class From
      {
         [Fact]
         public void ShouldSetBackupLocation()
         {
            var restorer = new Restore();

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
            var restorer = new Restore();

            const string expectedDatabaseName = "good.will";

            var actualRestorer = restorer.To(expectedDatabaseName);

            Assert.Equal(expectedDatabaseName, actualRestorer.DatabaseName);
         }
      }

      public class In
      {
         [Fact]
         public void ShouldSetDatabaseLocation()
         {
            var restorer = new Restore();

            const string expectedDatabaseLocation = @"~\Databases\good.will";

            var actualRestorer = restorer.In(expectedDatabaseLocation);

            Assert.Equal(expectedDatabaseLocation, actualRestorer.DatabaseLocation);
         }
      }

      public class At
      {
         [Fact]
         public void ShouldSetServerUrl()
         {
            var restorer = new Restore();

            const string expectedServerUrl = "http://good-will-hunting.org/";

            var actualRestorer = restorer.At(expectedServerUrl);

            Assert.Equal(expectedServerUrl, actualRestorer.ServerUrl);
         }
      }

      public class Run
      {
         [Fact]
         public void ShouldActuallyRestoreMyTestDb()
         {
            var restorer = new Restore()
               .From(@"C:\temp\test2.bak")
               .To("test2")
               .At("http://localhost:8080")
               //.In(@"C:\RavenDB\Databases\test")
               .Run();
      
         }
      }
   }
}