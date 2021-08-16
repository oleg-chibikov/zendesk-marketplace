using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OlegChibikov.ZendeskInterview.Marketplace.Contracts;

namespace OlegChibikov.ZendeskInterview.Marketplace.DAL
{
    public sealed class ResourceLoader<T> : IResourceLoader<T>
    {
        readonly string _fileNameWithoutExtension;

        public ResourceLoader(string fileNameWithoutExtension)
        {
            _fileNameWithoutExtension = fileNameWithoutExtension ?? throw new ArgumentNullException(nameof(fileNameWithoutExtension));
        }

        public async Task<IEnumerable<T>> LoadEntitiesAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"OlegChibikov.ZendeskInterview.Marketplace.DAL.Resources.{_fileNameWithoutExtension}.json";

            await using var stream = assembly.GetManifestResourceStream(resourceName) ?? throw new InvalidOperationException("Invalid resource: " + _fileNameWithoutExtension);
            using var streamReader = new StreamReader(stream);
            var json = await streamReader.ReadToEndAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<T>>(
                json,
                new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                }) ?? throw new InvalidOperationException("Deserialized value is null: " + _fileNameWithoutExtension);
        }
    }
}