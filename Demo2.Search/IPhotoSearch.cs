using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo2.Search
{
    public interface IPhotoSearch
    {
        Task CreateIndexes();
        Task Update(PhotoSearchItem item);
        Task<List<PhotoSearchItem>> Search(string text);
    }
}