namespace PrjFinanzas360.Interface
{
    public interface IOcrEngine
    {
        Task<string> ReadTextAsync(string imagePath);
    }
}
