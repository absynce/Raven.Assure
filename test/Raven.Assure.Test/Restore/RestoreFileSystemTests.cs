using Raven.Assure.Restore;
using Xunit;

namespace Raven.Assure.Test.Restore
{
   public class RestoreFileSystemTests
   {
      public class From
      {
         [Fact]
         public void ShouldSetBackupLocation()
         {
            var restorer = new RestoreFileSystem();

            const string expectedBackupLocation = "C:\\temp\\good.will";

            var actualRestorer = restorer.From(expectedBackupLocation);

            Assert.Equal(expectedBackupLocation, actualRestorer.BackupLocation);
         }
      }

      public class To
      {
         [Fact]
         public void ShouldSetFileSystemName()
         {
            var restorer = new RestoreFileSystem();

            const string expectedFileSystemName = "good.will";

            var actualRestorer = restorer.To(expectedFileSystemName);

            Assert.Equal(expectedFileSystemName, actualRestorer.FileSystemName);
         }
      }

      public class In
      {
         [Fact]
         public void ShouldSetRestoreLocation()
         {
            var restorer = new RestoreFileSystem();

            const string expectedRestoreLocation = @"~\FileSystems\good.will";

            var actualRestorer = restorer.In(expectedRestoreLocation);

            Assert.Equal(expectedRestoreLocation, actualRestorer.RestoreLocation);
         }
      }

      public class At
      {
         [Fact]
         public void ShouldSetServerUrl()
         {
            var restorer = new RestoreFileSystem();

            const string expectedServerUrl = "http://good-will-hunting.org/";

            var actualRestorer = restorer.At(expectedServerUrl);

            Assert.Equal(expectedServerUrl, actualRestorer.ServerUrl);
         }
      }

      public class Run
      {
         [Fact]
         //[Fact(Skip = "Manual testing only. Would need to pass store to allow testability.")]
         public void ShouldActuallyRestoreMyTestFileSystem()
         {
            new RestoreFileSystem()
               .From(@"C:\temp\test.files.raven.incremental.bak")
               .To("test.files.restored")
               .At("http://localhost:8080")
               //.In(@"C:\RavenDB\Databases\test")
               .Run();
         }
      }
   }
}