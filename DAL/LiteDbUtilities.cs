using System;
using LiteDB;

namespace OlegChibikov.ZendeskInterview.Marketplace.DAL
{
    public static class LiteDbUtilities
    {
        public static void RegisterTypes()
        {
            // TODO: DateTimeOffset is stored as Utc dateTime as it's not supported by LiteDb. With this storage we need to keep a separate TimeZone field for each DateTimeOffset property to display it as Original value, otherwise it's displayed as UTC date.
            BsonMapper.Global.RegisterType<DateTimeOffset>(
                (value) => new BsonValue(value.UtcDateTime),
                (bson) => bson.AsDateTime.ToUniversalTime());
        }
    }
}