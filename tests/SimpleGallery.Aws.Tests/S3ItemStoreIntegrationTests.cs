using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Logging;
using Moq;
using SimpleGallery.Core.Tests;
using Xunit;

namespace SimpleGallery.Aws.Tests
{
    public class S3ItemStoreIntegrationTests
    {
        private const string BucketName = "sam-testing-bucket";
        private static readonly TimeSpan LinkTtl = TimeSpan.FromHours(1);

        private readonly string[] _expectedKeys =
        {
            "1985-2009 Grace and Sam old Photos/Sam/2000-05-19 Ducks/Ducks1.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2000-05-19 Ducks/Ducks2.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2000-05-19 Ducks/Ducks3.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2000-05-19 Ducks/Ducks4.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2000-05-19 Ducks/Ducks5.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Dad at 6.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Dad.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Gemma.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Image010.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Image011.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Mum.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Nana.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/P1160011.JPG",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/P1170028.JPG",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Papa.jpg",
            "1985-2009 Grace and Sam old Photos/Sam/2004-12-25 Some Family Headshots/Tom.jpg",
            "2009-08-06 Will's Passing Out/P8050594.JPG",
            "2009-08-06 Will's Passing Out/P8050595.JPG",
            "2009-08-06 Will's Passing Out/P8050596.JPG",
            "2009-08-06 Will's Passing Out/P8050597.JPG",
            "2009-08-06 Will's Passing Out/P8050598.JPG",
            "2009-08-06 Will's Passing Out/P8050599.JPG",
            "2009-08-06 Will's Passing Out/P8050600.JPG",
            "2009-08-06 Will's Passing Out/P8050601.JPG",
            "2009-08-06 Will's Passing Out/P8050602.JPG",
            "2009-08-06 Will's Passing Out/P8060603.JPG",
            "2009-08-06 Will's Passing Out/P8060604.JPG",
            "2009-08-06 Will's Passing Out/P8060605.JPG",
            "2009-08-06 Will's Passing Out/P8060606.JPG",
            "2009-08-06 Will's Passing Out/P8060607.JPG",
            "2009-08-06 Will's Passing Out/P8060608.JPG"
        };

        private readonly RegionEndpoint _region = RegionEndpoint.EUWest1;
        private readonly string _testImage = "2009-08-06 Will's Passing Out/P8050597.JPG";

        [IntegrationFact]
        public async Task CanListBucket()
        {
            using (var s3 = new AmazonS3Client(_region))
            {
                var s3Handler = new S3ItemStore(s3, BucketName, LinkTtl, Mock.Of<ILogger>());

                var objects = await s3Handler.GetS3Objects("").ToList().ToTask();

                Assert.Equal(_expectedKeys.ToHashSet(), objects.Select(o => o.Key).ToHashSet());
            }
        }

        [IntegrationFact]
        public async Task CanListBucketAlbum()
        {
            var album = "2009-08-06 Will's Passing Out/";
            using (var s3 = new AmazonS3Client(_region))
            {
                var s3Handler = new S3ItemStore(s3, BucketName, LinkTtl, Mock.Of<ILogger>());

                var objects = await s3Handler.GetS3Objects(album).ToList().ToTask();

                Assert.Equal(_expectedKeys.Where(s => s.StartsWith(album)).ToHashSet(),
                    objects.Select(o => o.Key).ToHashSet());
            }
        }

        [IntegrationFact]
        public async Task CanGetObjectData()
        {
            using (var s3 = new AmazonS3Client(_region))
            using (var dest = new MemoryStream())
            {
                var s3Handler = new S3ItemStore(s3, BucketName, LinkTtl, Mock.Of<ILogger>());

                var output = await s3Handler.ReadItem(_testImage);
                output.CopyTo(dest);

                Assert.Equal(1108509, dest.Length);
            }
        }

        [IntegrationFact]
        public void SignsUrl()
        {
            using (var s3 = new AmazonS3Client(_region))
            {
                var s3Handler = new S3ItemStore(s3, BucketName, LinkTtl, Mock.Of<ILogger>());

                var url = s3Handler.UrlForPath(_testImage);
                
                Assert.StartsWith($"https://{BucketName}.s3-{_region.SystemName}.amazonaws.com/2009-08-06%20Will's%20Passing%20Out/P8050597.JPG?X-Amz", url);
            }
        }
    }
}