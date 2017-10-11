using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace SimpleGallery.Aws
{
    public interface IS3Handler
    {
        Task<Stream> ReadItem(string path);
        Task WriteItem(string path, Stream content);
        Task DeleteItem(string path);
        IObservable<S3Object> GetS3Objects(string path);
    }
}