using Raven.Assure.BackUp;
using Raven.Assure.Restore;

namespace Raven.Assure.Fluent
{
   public static class Restore
   {
      public static IRestoreDatabase<RestoreDatabase> Database()
      {
         return new RestoreDatabase();
      }

      public static IRestoreFileSystem<RestoreFileSystem> FileSystem()
      {
         return new RestoreFileSystem();
      }
   }
}