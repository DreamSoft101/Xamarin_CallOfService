using System;
using System.IO;

namespace CallOfService.Technician.Mobile.Droid.Services
{
    public class ImageCompressor : IImageCompressor
    {
        public Stream ResizeImage(Stream data, float width, float height)
        {
            throw new NotImplementedException();
        }

        public ImageMetaData GetImageMetaData(Stream imageData)
        {
            throw new NotImplementedException();
        }

        public Stream ResizeImage(Stream data, float percentage)
        {
            throw new NotImplementedException();
        }

        public byte[] ToArray(Stream input)
        {
            throw new NotImplementedException();
        }
    }
}