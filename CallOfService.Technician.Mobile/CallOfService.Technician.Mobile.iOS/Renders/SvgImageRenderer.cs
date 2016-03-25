﻿using System;
using System.IO;
using System.Reflection;
using CallOfService.Technician.Mobile.Controls;
using CallOfService.Technician.Mobile.iOS.Renders;
using NGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SvgImage), typeof(SvgImageRenderer))]

namespace CallOfService.Technician.Mobile.iOS.Renders
{
    public class SvgImageRenderer : ImageRenderer
    {


        private readonly Assembly _assemblyContainingImages = Assembly.Load(new AssemblyName("CallOfServiceTechnicianMobileiOS"));
        private readonly string _filePathsPrefix = "LabourCosting.iOS.Resources.Images.";
        private readonly string _filePathsSufix = ".svg";

        private SvgImage FormsControl => Element as SvgImage;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (FormsControl != null)
            {
                FormsControl.PropertyChanged += (sender, ev) => DrawImage();
            }
            DrawImage();

        }

        void DrawImage()
        {
            if (FormsControl != null && _assemblyContainingImages != null && FormsControl.Source != null)
            {
                var svgStream =
                    _assemblyContainingImages.GetManifestResourceStream($"{_filePathsPrefix}{FormsControl.FileName}{_filePathsSufix}");

                if (svgStream == null)
                {
                    throw new Exception(
                        $"Error retrieving {FormsControl.FileName} make sure Build Action is Embedded Resource");
                }

                var r = new SvgReader(new StreamReader(svgStream));

                var graphics = r.Graphic;

                var width = FormsControl.WidthRequest <= 0 ? 100 : FormsControl.WidthRequest;
                var height = FormsControl.HeightRequest <= 0 ? 100 : FormsControl.HeightRequest;

                var scale = 1.0;

                if (height >= width)
                {
                    scale = height / graphics.Size.Height;
                }
                else
                {
                    scale = width / graphics.Size.Width;
                }

                var scaleFactor = UIScreen.MainScreen.Scale;

                var canvas = new ApplePlatform().CreateImageCanvas(graphics.Size, scale * scaleFactor);
                graphics.Draw(canvas);
                var image = canvas.GetImage();

                var uiImage = image.GetUIImage();
                Control.Image = uiImage;
            }
        }
    }
}
