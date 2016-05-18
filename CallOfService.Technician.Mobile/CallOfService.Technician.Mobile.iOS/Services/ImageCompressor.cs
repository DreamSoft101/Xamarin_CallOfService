using System;
using System.IO;
using UIKit;
using System.Drawing;
using CoreGraphics;
using ImageIO;
using Foundation;

namespace CallOfService.Technician.Mobile.iOS.Services
{
	public class ImageCompressor : IImageCompressor
	{

		public Stream ResizeImage (Stream imageData, float width, float height)
		{
			var originalData = ToArray (imageData);

			var newData = ResizeImageIOS (originalData, width, height);

			return new MemoryStream (newData);
		}

		public Stream ResizeImage (Stream data, float percentage)
		{

			var originalImageMetaData = GetImageMetaData (data);

			float originalHeight = 0;
			float.TryParse (originalImageMetaData.Height, out originalHeight);

			float originalWidth = 0;
			float.TryParse (originalImageMetaData.Width, out originalWidth);


			float newHeight = originalHeight * percentage;
			float newWidth = originalWidth * percentage;

			var originalData = ToArray (data);

			var newData = ResizeImageIOS (originalData, newWidth, newHeight);

			return new MemoryStream (newData);
		}


		public byte[] ToArray (Stream input)
		{
			using (MemoryStream ms = new MemoryStream ()) {
				input.Position = 0;
				input.CopyTo (ms);
				input.Position = 0;
				return ms.ToArray ();
			}
		}

		private byte[] ResizeImageIOS (byte[] imageData, float width, float height)
		{
			UIImage originalImage = ImageFromByteArray ((imageData));

			RectangleF imageRect = new RectangleF (0, 0, width, height);

			UIGraphics.BeginImageContext (new CGSize (width, height));
			originalImage.Draw (imageRect);
			UIKit.UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();

			return resizedImage.AsJPEG ().ToArray ();
		}

		private UIKit.UIImage ImageFromByteArray (byte[] data)
		{
			if (data == null) {
				return null;
			}

			UIKit.UIImage image;
			try {
				image = new UIKit.UIImage (Foundation.NSData.FromArray (data));
			} catch (Exception e) {
				Console.WriteLine ("Image load failed: " + e.Message);
				return null;
			}
			return image;
		}

		public ImageMetaData GetImageMetaData (Stream imageData)
		{
			var originalData = ToArray (imageData);
			CGImageSource myImageSource = CGImageSource.FromData (NSData.FromArray (originalData));
			var ns = new NSDictionary ();
			var imageProperties = myImageSource.CopyProperties (ns, 0);
			var width = imageProperties [ImageIO.CGImageProperties.PixelWidth];
			var height = imageProperties [ImageIO.CGImageProperties.PixelHeight];
			var fileSize = imageProperties [ImageIO.CGImageProperties.FileSize];

			return new ImageMetaData () {
				DescriptionInStringsFileFormat = imageProperties.DescriptionInStringsFileFormat.Replace (" ", string.Empty),
				Width = width.ToString (),
				Height = height.ToString ()
			};
		}

	}
}

