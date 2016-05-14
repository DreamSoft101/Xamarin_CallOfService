using System;
using System.IO;

namespace CallOfService.Technician.Mobile
{
	public static class MediaExtensions
	{
		/// <summary>
		/// Verifies the options.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <exception cref="System.ArgumentNullException">options</exception>
		/// <exception cref="System.ArgumentException">options.Directory must be a relative folder;options</exception>
		public static void VerifyOptions(this MediaStorageOptions self)
		{
			if (self == null)
			{
				throw new ArgumentNullException("self");
			}
			//if (!Enum.IsDefined (typeof(MediaFileStoreLocation), options.Location))
			//    throw new ArgumentException ("options.Location is not a member of MediaFileStoreLocation");
			//if (options.Location == MediaFileStoreLocation.Local)
			//{
			//if (String.IsNullOrWhiteSpace (options.Directory))
			//	throw new ArgumentNullException ("options", "For local storage, options.Directory must be set");
			if (Path.IsPathRooted(self.Directory))
			{
				throw new ArgumentException("options.Directory must be a relative folder", "self");
			}
			//}
		}
	}
}

