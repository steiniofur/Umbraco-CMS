using Umbraco.Cms.Core.Models;

namespace Umbraco.Cms.Core.Media;

public sealed class NoopImageUrlGenerator : IImageUrlGenerator
{
    public IEnumerable<string> SupportedImageFileTypes { get; } = new[] { "jpg", "png", "gif" };

    public string? GetImageUrl(ImageUrlGenerationOptions options) => options?.ImageUrl;
}
