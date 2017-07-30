using Amazon.S3.Model;
using MediaStash.Lib.Services.Content;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fitcode.MediaStash.AmazonStorage
{
    // Expose Amazon specific details.
    public interface IAmazonMediaRepository : IMediaRepository
    {
        Task<IEnumerable<S3Object>> ListObjectRequest(string bucketName, string prefix);
    }
}
