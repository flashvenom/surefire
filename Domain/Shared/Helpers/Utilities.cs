using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.IO;
using System.Threading.Tasks;


namespace Mantis.Domain.Shared.Helpers
{
    public static class ImageResizer
    {
        public static async Task<MemoryStream> ResizeImageAsync(Stream imageStream, int maxSize)
        {
            using var image = await Image.LoadAsync(imageStream);
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxSize, maxSize)
            }));

            var memoryStream = new MemoryStream();
            await image.SaveAsJpegAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position
            return memoryStream;
        }
    }
}
