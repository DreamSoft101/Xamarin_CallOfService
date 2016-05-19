using System;
using System.IO;

namespace CallOfService.Mobile
{
	public static class MediaFileHelpers
	{
		/// <summary>
		/// Gets the output file with folder.
		/// </summary>
		/// <param name="isPhoto">if set to <c>true</c> [is photo].</param>
		/// <param name="folder">The root folder.</param>
		/// <param name="subdir">The subdir.</param>
		/// <param name="name">The name.</param>
		/// <returns>System.String.</returns>
		public static string GetMediaFileWithPath(bool isPhoto, string folder, string subdir, string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
				if (isPhoto)
				{
					name = "IMG_" + timestamp + ".jpg";
				}
				else
				{
					name = "VID_" + timestamp + ".mp4";
				}
			}

			var ext = Path.GetExtension(name);
			if (ext == String.Empty)
			{
				ext = ((isPhoto) ? ".jpg" : ".mp4");
			}

			name = Path.GetFileNameWithoutExtension(name);

			var newFolder = Path.Combine(folder ?? String.Empty, subdir ?? String.Empty);

			return Path.Combine(newFolder, name + ext);
		}
		public static string GetUniqueMediaFileWithPath(bool isPhoto, string folder, string name, Func<string, bool> checkExists)
		{
			var ext = Path.GetExtension(name);

			if (String.IsNullOrEmpty(ext))
			{
				ext = (isPhoto) ? ".jpg" : "mp4";
			}

			var nname = name + ext;
			var i = 1;
			while (checkExists(Path.Combine(folder, nname)))
			{
				nname = name + "_" + (i++) + ext;
			}

			return Path.Combine(folder, nname);
		}
	}
}

