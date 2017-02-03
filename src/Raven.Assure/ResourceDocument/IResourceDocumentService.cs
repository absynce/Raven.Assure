namespace Raven.Assure.ResourceDocument
{
   public interface IResourceDocumentService<TResourceDocument>
   {
      TResourceDocument FindLatest(string directory);
      string FindLatestDocumentPath(string directory);
      TResourceDocument Load(string documentPath);
      TResourceDocument Save(TResourceDocument document);
      ResourceDocumentUpdate<TResourceDocument> TryRemoveEncryptionKey(TResourceDocument document);
   }
}