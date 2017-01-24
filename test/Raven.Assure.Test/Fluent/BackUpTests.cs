using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Raven.Assure.Fluent;
using Raven.Assure.Log;
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
    }
}
