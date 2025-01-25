using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Surefire.Domain.Shared.Helpers
{
    public static class ImageResizer
    {
        public static async Task<MemoryStream> ResizeImageAsync(Stream imageStream, int maxSize)
        {
            using var image = await Image.LoadAsync(imageStream);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxSize, maxSize),
                Sampler = KnownResamplers.Lanczos3
            }));

            var memoryStream = new MemoryStream();
            await image.SaveAsJpegAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position
            return memoryStream;
        }

        public static async Task<MemoryStream> ResizeImagePngAsync(Stream imageStream, int maxSize)
        {
            using var image = await Image.LoadAsync(imageStream);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxSize, maxSize),
                Sampler = KnownResamplers.Lanczos3 // Use a high-quality resampling algorithm
            }));

            var memoryStream = new MemoryStream();
            await image.SaveAsPngAsync(memoryStream); // Save as PNG instead of JPEG
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position
            return memoryStream;
        }
    }
    
}
