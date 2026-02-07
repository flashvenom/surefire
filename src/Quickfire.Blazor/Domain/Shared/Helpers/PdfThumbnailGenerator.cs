using Docnet.Core;
using Docnet.Core.Models;
using iText.Kernel.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Quickfire.Blazor.Domain.Shared.Helpers;

public static class PdfThumbnailGenerator
{
    public static Task CreateJpegThumbnailAsync(
        string pdfFilePath,
        string thumbnailFilePath,
        int pageIndex = 0,
        int thumbnailWidth = 300,
        int jpegQuality = 80,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pdfFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(thumbnailFilePath);

        return CreateJpegThumbnailFromFileAsync(
            pdfFilePath,
            thumbnailFilePath,
            pageIndex,
            thumbnailWidth,
            jpegQuality,
            cancellationToken);
    }

    private static async Task CreateJpegThumbnailFromFileAsync(
        string pdfFilePath,
        string thumbnailFilePath,
        int pageIndex,
        int thumbnailWidth,
        int jpegQuality,
        CancellationToken cancellationToken)
    {
        using var stream = File.OpenRead(pdfFilePath);
        await CreateJpegThumbnailAsync(
            stream,
            thumbnailFilePath,
            pageIndex,
            thumbnailWidth,
            jpegQuality,
            cancellationToken);
    }

    public static Task CreateJpegThumbnailAsync(
        byte[] pdfBytes,
        string thumbnailFilePath,
        int pageIndex = 0,
        int thumbnailWidth = 300,
        int jpegQuality = 80,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfBytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(thumbnailFilePath);

        return CreateJpegThumbnailFromBytesAsync(
            pdfBytes,
            thumbnailFilePath,
            pageIndex,
            thumbnailWidth,
            jpegQuality,
            cancellationToken);
    }

    private static async Task CreateJpegThumbnailFromBytesAsync(
        byte[] pdfBytes,
        string thumbnailFilePath,
        int pageIndex,
        int thumbnailWidth,
        int jpegQuality,
        CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(pdfBytes, writable: false);
        await CreateJpegThumbnailAsync(
            stream,
            thumbnailFilePath,
            pageIndex,
            thumbnailWidth,
            jpegQuality,
            cancellationToken);
    }

    public static async Task CreateJpegThumbnailAsync(
        Stream pdfStream,
        string thumbnailFilePath,
        int pageIndex = 0,
        int thumbnailWidth = 300,
        int jpegQuality = 80,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(pdfStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(thumbnailFilePath);
        ArgumentOutOfRangeException.ThrowIfNegative(pageIndex);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(thumbnailWidth);

        if (jpegQuality is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(jpegQuality), "JPEG quality must be between 1 and 100.");
        }

        using var buffer = new MemoryStream();
        await pdfStream.CopyToAsync(buffer, cancellationToken);
        var pdfBytes = buffer.ToArray();

        var (targetWidth, targetHeight) = GetThumbnailDimensionsFromPdf(pdfBytes, pageIndex, thumbnailWidth);
        // Docnet PageDimensions expects the smaller dimension first.
        var dimOne = Math.Min(targetWidth, targetHeight);
        var dimTwo = Math.Max(targetWidth, targetHeight);

        var docLib = DocLib.Instance ?? throw new InvalidOperationException("Docnet could not initialize; pdfium might not be available at runtime.");
        using var docReader = docLib.GetDocReader(pdfBytes, new PageDimensions(dimOne, dimTwo));

        var pageCount = docReader.GetPageCount();
        if (pageCount <= 0)
        {
            throw new InvalidOperationException("PDF has no pages.");
        }

        if (pageIndex >= pageCount)
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index exceeds page count.");
        }

        using var pageReader = docReader.GetPageReader(pageIndex);
        var renderedBytes = pageReader.GetImage(RenderFlags.RenderAnnotations);

        // Some PDF render paths return a BGRX buffer where the alpha byte is always 0 (unused).
        // If we treat that as transparency and later save to JPEG, the result looks solid black.
        var hasAnyAlpha = false;
        for (var i = 3; i < renderedBytes.Length; i += 4)
        {
            if (renderedBytes[i] != 0)
            {
                hasAnyAlpha = true;
                break;
            }
        }

        if (!hasAnyAlpha)
        {
            for (var i = 3; i < renderedBytes.Length; i += 4)
            {
                renderedBytes[i] = 255;
            }
        }

        using var image = Image.LoadPixelData<Bgra32>(
            renderedBytes,
            pageReader.GetPageWidth(),
            pageReader.GetPageHeight());

        using var flattened = new Image<Rgba32>(image.Width, image.Height, Color.White);
        flattened.Mutate(ctx => ctx.DrawImage(image, 1f));

        if (flattened.Width != thumbnailWidth)
        {
            var resizedHeight = (int)Math.Max(1, Math.Round(flattened.Height * (thumbnailWidth / (double)flattened.Width)));
            flattened.Mutate(ctx => ctx.Resize(thumbnailWidth, resizedHeight, KnownResamplers.Lanczos3));
        }

        await flattened.SaveAsJpegAsync(
            thumbnailFilePath,
            new JpegEncoder { Quality = jpegQuality },
            cancellationToken);
    }

    private static (int width, int height) GetThumbnailDimensionsFromPdf(byte[] pdfBytes, int pageIndex, int thumbnailWidth)
    {
        if (pdfBytes.Length == 0)
        {
            throw new InvalidOperationException("PDF is empty.");
        }

        using var reader = new PdfReader(new MemoryStream(pdfBytes));
        using var document = new PdfDocument(reader);

        var pageNumber = pageIndex + 1;
        if (pageNumber < 1 || pageNumber > document.GetNumberOfPages())
        {
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index exceeds page count.");
        }

        var pageSize = document.GetPage(pageNumber).GetPageSizeWithRotation();
        var aspectRatio = pageSize.GetHeight() / pageSize.GetWidth();

        var targetHeight = (int)Math.Max(1, Math.Round(thumbnailWidth * aspectRatio));
        return (thumbnailWidth, targetHeight);
    }
}
