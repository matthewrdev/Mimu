using System;
namespace Mits
{
	public static class Constants
    {
        public const string XamlFileExtension = ".xaml";
        public const string CSharpFileExtension = ".cs";

        public static readonly IReadOnlyList<string> ImageFileExtensions = new List<string>()
        {
            ".png",
            ".jpeg",
            ".jpg",
            ".gif",
        };

        public const string ResourcesFolder = "Resources";

        public const string AppIconSetFolderExtension = ".appiconset";
        public const string ImageSetFolderExtension = ".imageset";

        public const string AssetSetFolderExtension = ".xcassets";

        public const string DrawableFolderPrefix = "drawable";

        public const string MipMapFolderPrefix = "mipmap";
    }
}

