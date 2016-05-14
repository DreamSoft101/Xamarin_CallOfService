using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CallOfService.Technician.Mobile.Controls;
using CallOfService.Technician.Mobile.Droid.Renderers;
using NGraphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Size = Xamarin.Forms.Size;

[assembly: ExportRenderer(typeof(SvgImage), typeof(SvgImageRenderer))]
namespace CallOfService.Technician.Mobile.Droid.Renderers
{
	[Preserve(AllMembers = true)]
	public class SvgImageRenderer : ViewRenderer<SvgImage, ImageView>
	{

		private readonly Assembly _assemblyContainingImages =
			Assembly.Load(new AssemblyName("CallOfService.Technician.Mobile.Droid"));

		private readonly string _filePathsPrefix = "CallOfService.Technician.Mobile.Droid.Resources.drawable.Images.";
		private readonly string _filePathsSufix = ".svg";

		public static void Init()
		{
		}

		private SvgImage FormsControl => Element as SvgImage;

		protected override void OnElementChanged(ElementChangedEventArgs<SvgImage> e)
		{
			base.OnElementChanged(e);
			if (FormsControl != null)
			{
				FormsControl.PropertyChanged += async (sender, ev) =>  DrawImage();
			}
			DrawImage();
		}

		private void DrawImage()
		{
			if (FormsControl != null)
			{
				var svgStream =
					_assemblyContainingImages.GetManifestResourceStream(
						$"{_filePathsPrefix}{FormsControl.FileName}{_filePathsSufix}");

				if (svgStream == null)
				{
					throw new Exception(
						$"Error retrieving {FormsControl.FileName} make sure Build Action is Embedded Resource");
				}

				var r = new SvgReader(new StreamReader(svgStream));

				var graphics = r.Graphic;


				var width = PixelToDP((int)FormsControl.WidthRequest <= 0 ? 100 : (int)FormsControl.WidthRequest);
				var height =
					PixelToDP((int)FormsControl.HeightRequest <= 0 ? 100 : (int)FormsControl.HeightRequest);

				var scale = 1.0;

				if (height >= width)
				{
					scale = height / graphics.Size.Height;
				}
				else
				{
					scale = width / graphics.Size.Width;
				}

				var canvas = new AndroidPlatform().CreateImageCanvas(graphics.Size, scale);
				graphics.Draw(canvas);
				var image = (BitmapImage)canvas.GetImage();

				Device.BeginInvokeOnMainThread(() =>
				{
					var imageView = new ImageView(Context);
					imageView.SetScaleType(ImageView.ScaleType.FitXy);
					imageView.SetImageBitmap(image.Bitmap);
					SetNativeControl(imageView);
				});
			}
		}

		public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
		{
			return new SizeRequest(new Size(FormsControl.WidthRequest, FormsControl.WidthRequest));
		}

		/// <summary>
		/// http://stackoverflow.com/questions/24465513/how-to-get-detect-screen-size-in-xamarin-forms
		/// </summary>
		/// <param name="pixel"></param>
		/// <returns></returns>
		private int PixelToDP(int pixel)
		{
			var scale = Resources.DisplayMetrics.Density;
			return (int)((pixel * scale) + 0.5f);
		}
	}
}