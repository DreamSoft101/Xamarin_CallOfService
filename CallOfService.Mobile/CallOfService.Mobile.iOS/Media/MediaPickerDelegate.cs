
using System;
using System.IO;
using System.Threading.Tasks;
using CallOfService.Mobile.iOS.Services;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CallOfService.Mobile.iOS
{
	internal class MediaPickerDelegate : UIImagePickerControllerDelegate
	{
		private UIDeviceOrientation? _orientation;
		private readonly NSObject _observer;
		private readonly MediaStorageOptions _options;
		private readonly UIImagePickerControllerSourceType _source;
		private readonly TaskCompletionSource<MediaFile> _tcs = new TaskCompletionSource<MediaFile>();
		private readonly UIViewController _viewController;

		internal MediaPickerDelegate(
			UIViewController viewController,
			UIImagePickerControllerSourceType sourceType,
			MediaStorageOptions options)
		{
			_viewController = viewController;
			_source = sourceType;
			_options = options ?? new CameraMediaStorageOptions();

			if (viewController != null)
			{
				UIDevice.CurrentDevice.BeginGeneratingDeviceOrientationNotifications();
				_observer = NSNotificationCenter.DefaultCenter.AddObserver(UIDevice.OrientationDidChangeNotification, DidRotate);
			}
		}

		public UIPopoverController Popover { get; set; }

		public UIView View
		{
			get
			{
				return _viewController.View;
			}
		}

		public Task<MediaFile> Task
		{
			get
			{
				return _tcs.Task;
			}
		}

		private bool IsCaptured
		{
			get
			{
				return _source == UIImagePickerControllerSourceType.Camera;
			}
		}

		public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
		{
			MediaFile mediaFile;
			switch ((NSString)info[UIImagePickerController.MediaType])
			{
				case MediaPicker.TypeImage:
					mediaFile = GetPictureMediaFile(info);
					break;

				case MediaPicker.TypeMovie:
					mediaFile = GetMovieMediaFile(info);
					break;

				default:
					throw new NotSupportedException();
			}

			Dismiss(picker, () => _tcs.TrySetResult(mediaFile));
		}

		public override void Canceled(UIImagePickerController picker)
		{
			Dismiss(picker, () => _tcs.TrySetCanceled());
		}

		public void DisplayPopover(bool hideFirst = false)
		{
			if (Popover == null)
			{
				return;
			}

			var swidth = UIScreen.MainScreen.Bounds.Width;
			var sheight = UIScreen.MainScreen.Bounds.Height;

			float width = 400;
			float height = 300;

			if (_orientation == null)
			{
				if (IsValidInterfaceOrientation(UIDevice.CurrentDevice.Orientation))
				{
					_orientation = UIDevice.CurrentDevice.Orientation;
				}
				else
				{
					_orientation = GetDeviceOrientation(_viewController.InterfaceOrientation);
				}
			}

			float x, y;
			if (_orientation == UIDeviceOrientation.LandscapeLeft || _orientation == UIDeviceOrientation.LandscapeRight)
			{
				y = (float)(swidth / 2) - (height / 2);
				x = (float)(sheight / 2) - (width / 2);
			}
			else
			{
				x = (float)(swidth / 2) - (width / 2);
				y = (float)(sheight / 2) - (height / 2);
			}

			if (hideFirst && Popover.PopoverVisible)
			{
				Popover.Dismiss(false);
			}

			Popover.PresentFromRect(new CGRect(x, y, width, height), View, 0, true);
		}

		private void Dismiss(UIImagePickerController picker, Action onDismiss)
		{
			if (_viewController == null)
			{
				onDismiss();
			}
			else
			{
				NSNotificationCenter.DefaultCenter.RemoveObserver(_observer);
				UIDevice.CurrentDevice.EndGeneratingDeviceOrientationNotifications();

				_observer.Dispose();

				if (Popover != null)
				{
					Popover.Dismiss(true);
					Popover.Dispose();
					Popover = null;

					onDismiss();
				}
				else
				{
					picker.DismissViewController(true, onDismiss);
					picker.Dispose();
				}
			}
		}

		private void DidRotate(NSNotification notice)
		{
			var device = (UIDevice)notice.Object;
			if (!IsValidInterfaceOrientation(device.Orientation) || Popover == null)
			{
				return;
			}
			if (_orientation.HasValue && IsSameOrientationKind(_orientation.Value, device.Orientation))
			{
				return;
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
			{
				if (!GetShouldRotate6(device.Orientation))
				{
					return;
				}
			}
			else if (!GetShouldRotate(device.Orientation))
			{
				return;
			}

			var co = _orientation;
			_orientation = device.Orientation;

			if (co == null)
			{
				return;
			}

			DisplayPopover(true);
		}

		private bool GetShouldRotate(UIDeviceOrientation orientation)
		{
			var iorientation = UIInterfaceOrientation.Portrait;
			switch (orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					iorientation = UIInterfaceOrientation.LandscapeLeft;
					break;

				case UIDeviceOrientation.LandscapeRight:
					iorientation = UIInterfaceOrientation.LandscapeRight;
					break;

				case UIDeviceOrientation.Portrait:
					iorientation = UIInterfaceOrientation.Portrait;
					break;

				case UIDeviceOrientation.PortraitUpsideDown:
					iorientation = UIInterfaceOrientation.PortraitUpsideDown;
					break;

				default:
					return false;
			}

			return _viewController.ShouldAutorotateToInterfaceOrientation(iorientation);
		}

		private bool GetShouldRotate6(UIDeviceOrientation orientation)
		{
			if (!_viewController.ShouldAutorotate())
			{
				return false;
			}

			var mask = UIInterfaceOrientationMask.Portrait;
			switch (orientation)
			{
				case UIDeviceOrientation.LandscapeLeft:
					mask = UIInterfaceOrientationMask.LandscapeLeft;
					break;

				case UIDeviceOrientation.LandscapeRight:
					mask = UIInterfaceOrientationMask.LandscapeRight;
					break;

				case UIDeviceOrientation.Portrait:
					mask = UIInterfaceOrientationMask.Portrait;
					break;

				case UIDeviceOrientation.PortraitUpsideDown:
					mask = UIInterfaceOrientationMask.PortraitUpsideDown;
					break;

				default:
					return false;
			}

			return _viewController.GetSupportedInterfaceOrientations().HasFlag(mask);
		}

		private MediaFile GetPictureMediaFile(NSDictionary info)
		{
			var image = (UIImage)info[UIImagePickerController.EditedImage];
			if (image == null)
			{
				image = (UIImage)info[UIImagePickerController.OriginalImage];
			}

			var path = GetOutputPath(
				MediaPicker.TypeImage,
				_options.Directory ?? ((IsCaptured) ? String.Empty : "temp"),
				_options.Name);

			using (var fs = File.OpenWrite(path))
			using (Stream s = new NsDataStream(image.AsJPEG()))
			{
				s.CopyTo(fs);
				fs.Flush();
			}

			Action<bool> dispose = null;
			if (_source != UIImagePickerControllerSourceType.Camera)
			{
				dispose = d => File.Delete(path);
			}

			return new MediaFile(path, () => File.OpenRead(path), dispose);
		}

		private MediaFile GetMovieMediaFile(NSDictionary info)
		{
			var url = (NSUrl)info[UIImagePickerController.MediaURL];

			var path = GetOutputPath(
				MediaPicker.TypeMovie,
				_options.Directory ?? ((IsCaptured) ? String.Empty : "temp"),
				_options.Name ?? Path.GetFileName(url.Path));

			File.Move(url.Path, path);

			Action<bool> dispose = null;
			if (_source != UIImagePickerControllerSourceType.Camera)
			{
				dispose = d => File.Delete(path);
			}

			return new MediaFile(path, () => File.OpenRead(path), dispose);
		}

		private static string GetUniquePath(string type, string path, string name)
		{
			var isPhoto = (type == MediaPicker.TypeImage);
			var ext = Path.GetExtension(name);
			if (ext == String.Empty)
			{
				ext = ((isPhoto) ? ".jpg" : ".mp4");
			}

			name = Path.GetFileNameWithoutExtension(name);

			var nname = name + ext;
			var i = 1;
			while (File.Exists(Path.Combine(path, nname)))
			{
				nname = name + "_" + (i++) + ext;
			}

			return Path.Combine(path, nname);
		}

		private static string GetOutputPath(string type, string path, string name)
		{
			path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), path);
			Directory.CreateDirectory(path);

			if (String.IsNullOrWhiteSpace(name))
			{
				var timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");
				if (type == MediaPicker.TypeImage)
				{
					name = "IMG_" + timestamp + ".jpg";
				}
				else
				{
					name = "VID_" + timestamp + ".mp4";
				}
			}

			return Path.Combine(path, GetUniquePath(type, path, name));
		}

		private static bool IsValidInterfaceOrientation(UIDeviceOrientation self)
		{
			return (self != UIDeviceOrientation.FaceUp && self != UIDeviceOrientation.FaceDown
					&& self != UIDeviceOrientation.Unknown);
		}

		private static bool IsSameOrientationKind(UIDeviceOrientation o1, UIDeviceOrientation o2)
		{
			if (o1 == UIDeviceOrientation.FaceDown || o1 == UIDeviceOrientation.FaceUp)
			{
				return (o2 == UIDeviceOrientation.FaceDown || o2 == UIDeviceOrientation.FaceUp);
			}
			if (o1 == UIDeviceOrientation.LandscapeLeft || o1 == UIDeviceOrientation.LandscapeRight)
			{
				return (o2 == UIDeviceOrientation.LandscapeLeft || o2 == UIDeviceOrientation.LandscapeRight);
			}
			if (o1 == UIDeviceOrientation.Portrait || o1 == UIDeviceOrientation.PortraitUpsideDown)
			{
				return (o2 == UIDeviceOrientation.Portrait || o2 == UIDeviceOrientation.PortraitUpsideDown);
			}

			return false;
		}

		private static UIDeviceOrientation GetDeviceOrientation(UIInterfaceOrientation self)
		{
			switch (self)
			{
				case UIInterfaceOrientation.LandscapeLeft:
					return UIDeviceOrientation.LandscapeLeft;
				case UIInterfaceOrientation.LandscapeRight:
					return UIDeviceOrientation.LandscapeRight;
				case UIInterfaceOrientation.Portrait:
					return UIDeviceOrientation.Portrait;
				case UIInterfaceOrientation.PortraitUpsideDown:
					return UIDeviceOrientation.PortraitUpsideDown;
				default:
					throw new InvalidOperationException();
			}
		}
	}
}

