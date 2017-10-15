using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleGallery.Aws;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaHandler;

namespace SimpleGallery.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            var thumbnailBucket = new S3Handler(s3Client, "sam-grace-photos-thumbs", TimeSpan.FromMinutes(10));
            var mediaBucket = new S3Handler(s3Client, "sam-grace-photos", TimeSpan.FromMinutes(10));
            var indexStore = new DynamoDbHandler(dynamoClient, "SamGracePhotosIndex");
            var mediaHandler = new CompositeMediaHandler
            {
                new PhotoHandler(new ImageSize(100, 100), 10),
            };
            var mediaStore = new AwsMediaStore(mediaBucket, thumbnailBucket, indexStore);
            GalleryBuilder = new GalleryBuilder<IAwsMediaItem, IAwsMediaItem, IAwsIndexItem<IAwsMediaItem>>(mediaStore, mediaHandler);
        }

        public IConfiguration Configuration { get; }
        public IGalleryBuilder GalleryBuilder { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Singleton(GalleryBuilder));
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