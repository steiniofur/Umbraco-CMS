// Copyright (c) Umbraco.
// See LICENSE for more details.

using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.Exceptions;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Media;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Umbraco.Cms.Core.PropertyEditors;

public sealed class RichTextEditorPastedImages
{
    private const string TemporaryImageDataAttribute = "data-tmpimg";
    private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly ILogger<RichTextEditorPastedImages> _logger;
    private readonly MediaFileManager _mediaFileManager;
    private readonly IMediaService _mediaService;
    private readonly MediaUrlGeneratorCollection _mediaUrlGenerators;
    private readonly IPublishedUrlProvider _publishedUrlProvider;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly string _tempFolderAbsolutePath;
    private readonly IImageUrlGenerator _imageUrlGenerator;
    private readonly ContentSettings _contentSettings;
    private readonly Dictionary<string, GuidUdi> _uploadedImages = new();

    public RichTextEditorPastedImages(
        IUmbracoContextAccessor umbracoContextAccessor,
        ILogger<RichTextEditorPastedImages> logger,
        IHostingEnvironment hostingEnvironment,
        IMediaService mediaService,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
        MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGenerators,
        IShortStringHelper shortStringHelper,
        IPublishedUrlProvider publishedUrlProvider,
        IImageUrlGenerator imageUrlGenerator,
        IOptions<ContentSettings> contentSettings)
    {
        _umbracoContextAccessor =
            umbracoContextAccessor ?? throw new ArgumentNullException(nameof(umbracoContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _hostingEnvironment = hostingEnvironment;
        _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider ??
                                          throw new ArgumentNullException(nameof(contentTypeBaseServiceProvider));
        _mediaFileManager = mediaFileManager;
        _mediaUrlGenerators = mediaUrlGenerators;
        _shortStringHelper = shortStringHelper;
        _publishedUrlProvider = publishedUrlProvider;
        _imageUrlGenerator = imageUrlGenerator;
        _contentSettings = contentSettings.Value;

        _tempFolderAbsolutePath = _hostingEnvironment.MapPathContentRoot(Constants.SystemDirectories.TempImageUploads);
    }

    /// <summary>
    /// Used by the RTE (and grid RTE) for converting inline base64 images to Media items
    /// </summary>
    /// <param name="html"></param>
    /// <param name="mediaParentFolder"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException">Thrown if image extension is not allowed</exception>
    internal string FindAndPersistBase64Images(string html, Guid mediaParentFolder, int userId)
    {
        // Find all img's that has data-tmpimg attribute
        // Use HTML Agility Pack - https://html-agility-pack.net
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        HtmlNodeCollection? imagesWithDataUris = htmlDoc.DocumentNode.SelectNodes("//img");
        if (imagesWithDataUris == null || imagesWithDataUris.Count == 0)
        {
            return html;
        }

        foreach (HtmlNode? img in imagesWithDataUris)
        {
            var srcValue = img.GetAttributeValue("src", string.Empty);

            // Ignore src-less images
            if (string.IsNullOrEmpty(srcValue))
            {
                continue;
            }

            // Take only images that have a "data:image" uri into consideration
            if (!srcValue.StartsWith("data:image"))
            {
                continue;
            }

            // Create tmp image by scanning the srcValue
            // the value will look like "data:image/jpg;base64,abc" where the first part
            // is the mimetype and the second (after the comma) is the image blob
            var tokens = srcValue.Split(',');
            var dataUriInfo = tokens[0];
            var mimeType = dataUriInfo.Split(';')[0].Replace("data:", string.Empty);
            var ext = mimeType[(mimeType.LastIndexOf('/') + 1)..].ToLowerInvariant();
            var base64ImageString = tokens[1];

            if (_contentSettings.IsFileAllowedForUpload(ext) == false ||
                _imageUrlGenerator.IsSupportedImageFormat(ext) == false)
            {
                // Throw some error - to say can't upload this IMG type
                throw new NotSupportedException("This is not an image filetype extension that is approved");
            }

            // Create an unique folder path to help with concurrent users to avoid filename clash
            var imageTempPath =
                _hostingEnvironment.MapPathContentRoot(Constants.SystemDirectories.TempImageUploads + Path.DirectorySeparatorChar + Guid.NewGuid());

            // Ensure image temp path exists
            if (Directory.Exists(imageTempPath) == false)
            {
                Directory.CreateDirectory(imageTempPath);
            }

            // To get the filename, we simply manipulate the mimetype into a filename
            var filePath = mimeType.Replace('/', '.');
            var safeFileName = filePath.ToSafeFileName(_shortStringHelper);
            var tmpImgPath = imageTempPath + Path.DirectorySeparatorChar + safeFileName;
            var absoluteTempImagePath = Path.GetFullPath(tmpImgPath);

            // Convert the base64 content to a byte array and save the bytes directly to a file
            // this method should work for most use-cases
            System.IO.File.WriteAllBytes(absoluteTempImagePath, Convert.FromBase64String(base64ImageString));

            // When the temp file has been created, we can persist it
            PersistMediaItem(mediaParentFolder, userId, img, tmpImgPath);
        }

        return htmlDoc.DocumentNode.OuterHtml;
    }

    /// <summary>
    ///     Used by the RTE (and grid RTE) for drag/drop/persisting images
    /// </summary>
    public string FindAndPersistPastedTempImages(string html, Guid mediaParentFolder, int userId)
    {
        // Find all img's that has data-tmpimg attribute
        // Use HTML Agility Pack - https://html-agility-pack.net
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        HtmlNodeCollection? tmpImages = htmlDoc.DocumentNode.SelectNodes($"//img[@{TemporaryImageDataAttribute}]");
        if (tmpImages == null || tmpImages.Count == 0)
        {
            return html;
        }

        foreach (HtmlNode? img in tmpImages)
        {
            // The data attribute contains the path to the tmp img to persist as a media item
            var tmpImgPath = img.GetAttributeValue(TemporaryImageDataAttribute, string.Empty);

            if (string.IsNullOrEmpty(tmpImgPath))
            {
                continue;
            }

            var qualifiedTmpImgPath = _hostingEnvironment.MapPathContentRoot(tmpImgPath);

            PersistMediaItem(mediaParentFolder, userId, img, qualifiedTmpImgPath);
        }

        return htmlDoc.DocumentNode.OuterHtml;
    }

    private void PersistMediaItem(Guid mediaParentFolder, int userId, HtmlNode img, string qualifiedTmpImgPath)
    {
        var absoluteTempImagePath = Path.GetFullPath(qualifiedTmpImgPath);

        if (IsValidPath(absoluteTempImagePath) == false)
        {
            return;
        }

        var fileName = Path.GetFileName(absoluteTempImagePath);
        var safeFileName = fileName.ToSafeFileName(_shortStringHelper);

        var mediaItemName = safeFileName.ToFriendlyName();
        GuidUdi udi;

        if (_uploadedImages.ContainsKey(qualifiedTmpImgPath) == false)
        {
            IMedia mediaFile;
            if (mediaParentFolder == Guid.Empty)
            {
                mediaFile = _mediaService.CreateMedia(mediaItemName, Constants.System.Root,
                    Constants.Conventions.MediaTypes.Image, userId);
            }
            else
            {
                mediaFile = _mediaService.CreateMedia(mediaItemName, mediaParentFolder,
                    Constants.Conventions.MediaTypes.Image, userId);
            }

            var fileInfo = new FileInfo(absoluteTempImagePath);

            FileStream? fileStream = fileInfo.OpenReadWithRetry();
            if (fileStream == null)
            {
                throw new InvalidOperationException("Could not acquire file stream");
            }

            using (fileStream)
            {
                mediaFile.SetValue(_mediaFileManager, _mediaUrlGenerators, _shortStringHelper,
                    _contentTypeBaseServiceProvider, Constants.Conventions.Media.File, safeFileName, fileStream);
            }

            _mediaService.Save(mediaFile, userId);

            udi = mediaFile.GetUdi();
        }
        else
        {
            // Already been uploaded & we have it's UDI
            udi = _uploadedImages[qualifiedTmpImgPath];
        }

        // Add the UDI to the img element as new data attribute
        img.SetAttributeValue("data-udi", udi.ToString());

        // Get the new persisted image URL
        _umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext);
        IPublishedContent? mediaTyped = umbracoContext?.Media?.GetById(udi.Guid);
        if (mediaTyped == null)
        {
            throw new PanicException(
                $"Could not find media by id {udi.Guid} or there was no UmbracoContext available.");
        }

        var location = mediaTyped.Url(_publishedUrlProvider);

        // Find the width & height attributes as we need to set the imageprocessor QueryString
        var width = img.GetAttributeValue("width", int.MinValue);
        var height = img.GetAttributeValue("height", int.MinValue);

        if (width != int.MinValue && height != int.MinValue)
        {
            location = _imageUrlGenerator.GetImageUrl(new ImageUrlGenerationOptions(location)
            {
                ImageCropMode = ImageCropMode.Max,
                Width = width,
                Height = height,
            });
        }

        img.SetAttributeValue("src", location);

        // Remove the data attribute (so we do not re-process this)
        img.Attributes.Remove(TemporaryImageDataAttribute);

        // Add to the dictionary to avoid dupes
        if (_uploadedImages.ContainsKey(qualifiedTmpImgPath) == false)
        {
            _uploadedImages.Add(qualifiedTmpImgPath, udi);

            // Delete folder & image now its saved in media
            // The folder should contain one image - as a unique guid folder created
            // for each image uploaded from TinyMceController
            var folderName = Path.GetDirectoryName(absoluteTempImagePath);
            try
            {
                if (folderName is not null)
                {
                    Directory.Delete(folderName, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not delete temp file or folder {FileName}", absoluteTempImagePath);
            }
        }
    }

    private bool IsValidPath(string imagePath)
    {
        return imagePath.StartsWith(_tempFolderAbsolutePath);
    }
}
