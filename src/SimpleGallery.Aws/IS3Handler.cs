using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace SimpleGallery.Aws
{
    public interface IS3Handler
    {
        Task ReadItem(string path, Stream output);
        IObservable<S3Object> GetS3Objects(string path);
    }
}