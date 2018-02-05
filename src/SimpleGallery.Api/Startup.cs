using System;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleGallery.Api.Models;
using SimpleGallery.Aws;
using SimpleGallery.Aws.Model;
using SimpleGallery.Core;
using SimpleGallery.Core.Model;
using SimpleGallery.Core.Model.MediaPreprocessor;

namespace SimpleGallery.Api
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Startup
    {
        private readonly ILogger _logger;

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;

            _logger = loggerFactory.CreateLogger("Startup");
            _logger.LogInformation("Starting...");

            _logger.LogDebug("Creating AWS Clients");
            var s3Client = new AmazonS3Client(RegionEndpoint.EUWest1);
            var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.EUWest1);
            
            _logger.LogDebug("Instantiating AWS Adapters");
            var thumbnailBucket = new S3ItemStore(s3Client, "sam-grace-photos-thumbs", TimeSpan.FromMinutes(10), loggerFactory.CreateLogger("S3Thumbnails"));
            var mediaBucket = new S3ItemStore(s3Client, "sam-grace-photos", TimeSpan.FromMinutes(10), loggerFactory.CreateLogger("S3Images"));
            var indexStore = new DynamoDbIndex(dynamoClient, "SamGracePhotosIndex", loggerFactory.CreateLogger<DynamoDbIndex>());
            
            _logger.LogDebug("Instantiating Image Preprocessors");
            var mediaHandler = new CompositeMediaPreprocessor
            {
                new PhotoPreprocessor(new ImageSize(200, 200), 10, loggerFactory.CreateLogger<PhotoPreprocessor>()),
            };
            
            _logger.LogDebug("Instantiating MediaStore");
            var mediaStore = new AwsMediaStore(mediaBucket, thumbnailBucket, indexStore, loggerFactory.CreateLogger<AwsMediaStore>());
            _logger.LogDebug("Instantiating GalleryBuilder");
            GalleryBuilder = new GalleryBuilder<IAwsMediaItem, IAwsMediaItem, IAwsIndexItem<IAwsMediaItem>>(mediaStore, mediaHandler, loggerFactory.CreateLogger("GalleryBuilder"));
            _logger.LogDebug("Instantiating GalleryViewer");
            GalleryViewer = new GalleryViewer<IAwsMediaItem, IAwsMediaItem, IAwsIndexItem<IAwsMediaItem>>(mediaStore, TimeSpan.FromMinutes(5), loggerFactory.CreateLogger("GalleryViewer"));
        }

        private IConfiguration Configuration { get; }
        private IGalleryBuilder GalleryBuilder { get; }
        private IGalleryViewer GalleryViewer { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger.LogDebug("Configuring Options...");
            services.AddOptions();
            services.Configure<AppOptions>(Configuration.GetSection("App"));
            
            _logger.LogDebug("Configuring Services...");
            services.Add(ServiceDescriptor.Singleton(GalleryBuilder));
            services.AddSingleton<IHostedService, GalleryBuilderService>();
            services.Add(ServiceDescriptor.Singleton(GalleryViewer));
            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            _logger.LogDebug("Configure App...");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}