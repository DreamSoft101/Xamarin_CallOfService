using System.IO;
using Foundation;

namespace CallOfService.Mobile.iOS
{
	internal unsafe class NsDataStream : UnmanagedMemoryStream
	{
		private readonly NSData _data;

		public NsDataStream(NSData data)
			: base((byte*)data.Bytes, (long)data.Length)
		{
			_data = data;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_data.Dispose();
			}

			base.Dispose(disposing);
		}
	}
}

