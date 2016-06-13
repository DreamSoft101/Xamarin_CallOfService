using System;
using System.IO;
using System.Reflection;
using Android.Graphics;
using Android.Runtime;
using CallOfService.Mobile.Droid.Renderers;
using NGraphics;
using TwinTechs.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Size = NGraphics.Size;
using SvgImage = CallOfService.Mobile.Controls.SvgImage;

[assembly: ExportRenderer(typeof(SvgImage), typeof(SvgImageRenderer))]
namespace CallOfService.Mobile.Droid.Renderers
{
    [Preserve(AllMembers = true)]
    public class SvgImageRenderer : ImageRenderer
    {
        private readonly Assembly _assemblyContainingImages = typeof(App).GetTypeInfo().Assembly;
        private readonly string _filePathsPrefix = "CallOfService.Mobile.Resources.Images.";
        private readonly string _filePathsSufix = ".svg";

        public SvgImageRenderer()
        {
            // Offer to do our own drawing so Android will actually call `Draw`.
            SetWillNotDraw(false);
        }

        SvgImage FormsControl
        {
            get
            {
                return Element as SvgImage;
            }
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);

            if (FormsControl != null)
            {
                const double screenScale = 1.0; // Don't need to deal with screen scaling on Android.

                var finalCanvas = RenderSvgToCanvas(new Size(canvas.Width, canvas.Height), screenScale);
                var image = (BitmapImage)finalCanvas.GetImage();

                Control.SetImageBitmap(image.Bitmap);
            }
        }

        protected IImageCanvas RenderSvgToCanvas(Size outputSize, double finalScale)
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

            var originalSvgSize = graphics.Size;
            var finalCanvas = new AndroidPlatform().CreateImageCanvas(outputSize, finalScale);

            double proportionalOutputScale = originalSvgSize.ScaleThatFits(outputSize);

            graphics.ViewBox = new NGraphics.Rect(NGraphics.Point.Zero, graphics.Size / proportionalOutputScale);
            graphics.Draw(finalCanvas);

            return finalCanvas;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            Invalidate();
        }
    }
}