using System.IO.Abstractions;
using Raven.Assure.Log;

namespace Raven.Assure.Fluent
{
   public class AssureDataBase : IAssureDataBase, ISetupAssure<AssureDataBase>
   {
      protected IFileSystem fileSystem;
      protected ILogger logger;

      public string BackupLocation { get; protected set; }
      public string DatabaseName { get; protected set; }
      public string ServerUrl { get; protected set; }

      public AssureDataBase()
      {
         this.fileSystem = new FileSystem();
         this.logger = new ConsoleLogger();
      }

      public AssureDataBase LogWith(ILogger logger)
      {
         this.logger = logger;

         return this;
      }

      public AssureDataBase On(IFileSystem fileSystem)
      {
         this.fileSystem = fileSystem;

         return this;
      }
   }
}
