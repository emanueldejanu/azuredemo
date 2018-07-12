using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Configuration;

namespace Demo2.Search
{
    public class PhotoSearch : IPhotoSearch
    {
        private const string PhotosIndexName = "photos";
        private readonly IConfiguration configuration;
        private SearchServiceClient serviceClient;

        public PhotoSearch(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private void CreateSearchServiceClient()
        {
            if (serviceClient != null)
                return;

            string searchServiceName = configuration["SearchServiceName"];
            string adminApiKey = configuration["SearchServiceAdminApiKey"];

            serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(adminApiKey));
        }

        public async Task CreateIndexes()
        {
            CreateSearchServiceClient();
            if (await serviceClient.Indexes.ExistsAsync(PhotosIndexName))
                return;

            var definition = new Index
            {
                Name = PhotosIndexName,
                Fields = FieldBuilder.BuildForType<PhotoSearchItem>()
            };

            await serviceClient.Indexes.CreateAsync(definition);
        }

        public async Task Update(PhotoSearchItem item)
        {
            CreateSearchServiceClient();
            ISearchIndexClient indexClient = serviceClient.Indexes.GetClient(PhotosIndexName);
            var actions = new[]
            {
                IndexAction.MergeOrUpload(item)
            };

            var batch = IndexBatch.New(actions);
            await indexClient.Documents.IndexAsync(batch);
        }

        public async Task<List<PhotoSearchItem>> Search(string text)
        {
            string searchServiceName = configuration["SearchServiceName"];
            string queryApiKey = configuration["SearchServiceQueryApiKey"];

            SearchIndexClient indexClient = new SearchIndexClient(searchServiceName, PhotosIndexName, new SearchCredentials(queryApiKey));
            var parameters = new SearchParameters();

            DocumentSearchResult<PhotoSearchItem> results = 
                await indexClient.Documents.SearchAsync<PhotoSearchItem>(text, parameters);

            return results.Results.Select(x => x.Document).ToList();
        }
    }
}
