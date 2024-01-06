﻿using FusionRipper.Core.Data;
using FusionRipper.Core.Data.PackageReaders;
using FusionRipper.Core.Memory;
using System.Drawing;

namespace FusionRipper.Core.FileReaders
{
    public class ANMFileReader : IFileReader
    {
        public string Name => "ANM";
        public Dictionary<int, Bitmap> Icons { get { return _icons; } set { _icons = value; } }
        private Dictionary<int, Bitmap> _icons = new Dictionary<int, Bitmap>();

        public ANMPackageData Package = new();

        public void LoadGame(ByteReader fileReader, string filePath)
        {
            Package.AppName = Path.GetFileNameWithoutExtension(filePath);
            Package.Read(fileReader);
        }

        public PackageData getPackageData() => Package!;

        public IFileReader Copy()
        {
            ANMFileReader fileReader = new()
            {
                Package = Package,
                Icons = _icons
            };
            return fileReader;
        }
    }
}