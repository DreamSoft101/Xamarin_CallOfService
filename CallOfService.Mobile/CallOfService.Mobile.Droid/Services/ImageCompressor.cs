using System.IO;
using Android.Graphics;

namespace CallOfService.Mobile.Droid.Services
{
    public class ImageCompressor : IImageCompressor
    {
        public Stream ResizeImage(Stream data, float width, float height)
        {
			var imageData = ToArray(data);

			Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);

			using (MemoryStream ms = new MemoryStream())
			{
				resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
				return ms;
			}
        }

        public ImageMetaData GetImageMetaData(Stream imageData)
        {
            var original = BitmapFactory.DecodeStream(imageData);
			return new ImageMetaData()
			{
				Width = original.Width.ToString(),
				Height = original.Height.ToString(),
			};
        }

        public Stream ResizeImage(Stream data, float percentage)
        {
			var originalImageMetaData = GetImageMetaData(data);

			float originalHeight = 0;
			float.TryParse(originalImageMetaData.Height, out originalHeight);

			float originalWidth = 0;
			float.TryParse(originalImageMetaData.Width, out originalWidth);


			float newHeight = originalHeight * percentage;
			float newWidth = originalWidth * percentage;

			var imageData = ToArray(data);

			Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
			Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

			MemoryStream ms = new MemoryStream();
			resizedImage.Compress(Bitmap.CompressFormat.Jpeg, 100, ms);
			return ms;
        }

        public byte[] ToArray(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
			{
				input.Position = 0;
				input.CopyTo(ms);
				input.Position = 0;
				return ms.ToArray();
			}
        }
    }
}