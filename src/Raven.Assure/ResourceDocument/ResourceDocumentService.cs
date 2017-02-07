using System;
using System.IO;
using System.IO.Abstractions;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using Raven.Abstractions.Extensions;
using System.Linq;
using Raven.Abstractions.FileSystem;
using Raven.Imports.Newtonsoft.Json;

namespace Raven.Assure.ResourceDocument
{
   public abstract class ResourceDocumentService<TResourceDocument> : IResourceDocumentService<TResourceDocument>
   {
      private readonly IFileSystem fileSystem;

      public ResourceDocumentService(IFileSystem fileSystem)
      {
         this.fileSystem = fileSystem;
      }

      public TResourceDocument FindLatest(string directory)
      {
         var documentPath = FindLatestDocumentPath(directory);
         return Load(documentPath);
      }

      /// <summary>
      /// </summary>
      /// <remarks>Adapted from RavenDB@3.0.30155 - Raven.Database.Actions.MaintenanceActions.FindDatabaseDocument.</remarks>
      /// <param name="backupLocation"></param>
      /// <returns></returns>
      public string FindLatestDocumentPath(string backupLocation)
      {
         var documentFilename = getResourceDocumentFileName();
         var backupPath = fileSystem.Directory.GetDirectories(backupLocation, "Inc*")
                           .OrderByDescending(dir => dir)
                           .Select(dir => Path.Combine(dir, documentFilename))
                           .FirstOrDefault();

         return backupPath ?? Path.Combine(backupLocation, documentFilename);
      }

      public TResourceDocument Load(string documentPath)
      {
         if (!fileSystem.File.Exists(documentPath))
         {
            return default(TResourceDocument);
         }

         var databaseDocumentText = fileSystem.File.ReadAllText(documentPath);

         return RavenJObject.Parse(databaseDocumentText).JsonDeserialization<TResourceDocument>();
      }

      public TResourceDocument Save(TResourceDocument document, string documentPath)
      {
         var databaseDocumentText = JsonConvert.SerializeObject(document);
         fileSystem.File.WriteAllText(documentPath, databaseDocumentText);

         return document;
      }

      public abstract ResourceDocumentUpdate<TResourceDocument> TryRemoveEncryptionKey(TResourceDocument document);

      protected abstract string getResourceDocumentFileName();
   }
}