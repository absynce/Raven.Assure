using Raven.Assure.BackUp;

namespace Raven.Assure.Fluent
{
   public static class BackUp
   {
      public static IBackUpDatabase<BackUpDatabase> Database()
      {
         return new BackUpDatabase();
      }

      //public static IBackUpFileSystem<BackUpFileSystem> FileSystem() { }
   }
}