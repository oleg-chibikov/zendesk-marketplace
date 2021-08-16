namespace OlegChibikov.ZendeskInterview.Marketplace.Contracts
{
    public interface IRelatedEntitiesLoader
    {
        void LoadRelatedEntities(object mainEntity);
    }
}