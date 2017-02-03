namespace Raven.Assure.BackUp
{
   public interface IBackUp<out TInterface>
   {
      TInterface At(string url);
      TInterface Incrementally(bool incremental = true);
   }
}