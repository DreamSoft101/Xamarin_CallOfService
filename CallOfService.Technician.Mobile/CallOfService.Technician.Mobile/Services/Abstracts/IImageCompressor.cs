using System;
using System.IO;

namespace CallOfService.Technician.Mobile
{
	public interface IImageCompressor
	{
		Stream ResizeImage(Stream data, float width, float height);

		ImageMetaData GetImageMetaData (Stream imageData);

		Stream ResizeImage(Stream data, float percentage);

		byte[] ToArray (Stream input);
	}
}

