# SimpleGallery
### An Image Gallery without the fuss.

SimpleGallery is a project born our of fustration to find an image gallery that works out of S3.

When decommissioning my home network attached storage I moved all of my photos to AWS S3. It is easy, cheap, safe and convenient... Except there is then no easy way to view them.

I decided to create a thin photo gallery that would not require any effort to use. It will:
* Monitor an S3 bucket for the authoritive source of full-resolution images.
* Automatically index and build a gallery database, using path structure to determine albums.
* Generate and maintain thumbnail images.
* Only provide a thin web API.
    * Allowing for a web UI to be hosted in another S3 bucket.
    * Generate signed S3 URLs for images, saving processing.
* Work with videos and other future expansions.
