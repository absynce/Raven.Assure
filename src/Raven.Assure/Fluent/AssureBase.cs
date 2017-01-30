﻿using System.IO.Abstractions;
using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public class AssureBase : IAssureBase, ISetupAssure<AssureBase>
   {
      protected IFileSystem fileSystem;
      protected ILogger logger;

      public string BackupLocation { get; protected set; }
      public string DatabaseName { get; protected set; }
      public string ServerUrl { get; protected set; }

      public AssureBase()
      {
         this.fileSystem = new FileSystem();
         this.logger = new ConsoleLogger();
      }

      public AssureBase LogWith(ILogger logger)
      {
         this.logger = logger;

         return this;
      }

      public AssureBase On(IFileSystem fileSystem)
      {
         this.fileSystem = fileSystem;

         return this;
      }
   }
}
