using Xunit;

namespace Raven.Assure.Test.Fluent
{
   public class AssureBaseTests
   {
      public class LogWith
      {
         [Fact]
         public void ShouldSetLogger()
         {
            // Not sure how to test since logger is protected and base doesn't use it explicitly.
         }
      }
   }
}