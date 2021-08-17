namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface IOutputFormatter
    {
        string ListProperties(object? obj, int prefixSpaces = 0);
    }
}