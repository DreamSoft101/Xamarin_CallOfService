using System;
using UIKit;

namespace CallOfService.Mobile.iOS
{
	internal class MediaPickerPopoverDelegate : UIPopoverControllerDelegate
	{
		private readonly UIImagePickerController _picker;
		private readonly MediaPickerDelegate _pickerDelegate;

		internal MediaPickerPopoverDelegate(MediaPickerDelegate pickerDelegate, UIImagePickerController picker)
		{
			_pickerDelegate = pickerDelegate;
			_picker = picker;
		}

		public override bool ShouldDismiss(UIPopoverController popoverController)
		{
			return true;
		}

		public override void DidDismiss(UIPopoverController popoverController)
		{
			_pickerDelegate.Canceled(_picker);
		}
	}
}

