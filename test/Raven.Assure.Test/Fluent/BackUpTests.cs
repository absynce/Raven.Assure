﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
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
    }
}
