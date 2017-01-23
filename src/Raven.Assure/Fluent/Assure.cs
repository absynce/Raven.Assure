using System;

namespace Raven.Assure.Fluent
{
   public static class Assure
   {
      public static IBackUp BackUp()
      {
         throw new NotImplementedException("Need to back up.");
      }

      public static IRestore<Restore>  Restore()
      {
         throw new NotImplementedException("Need to restore.");
      }
   }
}