using System;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace CallOfService.Technician.Mobile.iOS
{
public sealed class MediaPickerController : UIImagePickerController
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MediaPickerController"/> class.
		/// </summary>
		/// <param name="mpDelegate">The mp delegate.</param>
		internal MediaPickerController(MediaPickerDelegate mpDelegate)
		{
			base.Delegate = mpDelegate;
		}

		/// <summary>
		/// Gets or sets the delegate.
		/// </summary>
		/// <value>The delegate.</value>
		/// <exception cref="NotSupportedException"></exception>
		public override NSObject Delegate
		{
			get
			{
				return base.Delegate;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Gets the result asynchronous.
		/// </summary>
		/// <returns>Task&lt;MediaFile&gt;.</returns>
		public Task<MediaFile> GetResultAsync()
		{
			return ((MediaPickerDelegate)Delegate).Task;
		}
	}
}

