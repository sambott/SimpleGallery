using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleGallery.Aws;
using SimpleGallery.Core;
using SimpleGallery.Core.Media;
using SimpleGallery.Core.Media.MediaHandler;

namespace SimpleGallery.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            var thumbnailBucket = new S3Handler(s3Client, "sam-grace-photos-thumbs");
            var mediaBucket = new S3Handler(s3Client, "sam-grace-photos");
            var indexStore = new DynamoDbHandler(dynamoClient, "SamGracePhotosIndex");
            var mediaHandler = new CompositeMediaHandler
            {
                new PhotoHandler(new ImageSize(100, 100), 10),
            };
            MediaStore = new AwsMediaStore(mediaHandler, mediaBucket, thumbnailBucket, indexStore);
        }

        public IConfiguration Configuration { get; }
        public IMediaStore MediaStore { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Singleton<IMediaStore>(MediaStore));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}