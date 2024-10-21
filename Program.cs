using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace OrdenaFotos
{
	public class ExempleExif
	{
		public static DateTime? ObtenirDataExif(string imagePath)
		{
			try
			{
				// Llegeix tots els directors de metadades de la imatge
				var directories = ImageMetadataReader.ReadMetadata(imagePath);

				// Busca la data de captura en el directori EXIF
				foreach (var directory in directories)
				{
					if (directory is ExifSubIfdDirectory exifDirectory)
					{
						// Busca el tag corresponent a la data de captura
						DateTime? dateTaken = exifDirectory.GetDateTime(ExifDirectoryBase.TagDateTimeOriginal);
						if (dateTaken.HasValue)
						{
							return dateTaken;
						}
					}
				}

				Console.WriteLine("No s'ha trobat la data EXIF a la imatge.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error llegint EXIF: {ex.Message}");
			}

			return null;
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			string inboxDirectory = @"C:\Users\yordy\Documents\dev\bootcamp\.net\OrdenaFotos\inbox";
			string photoLibraryDirectory = @"C:\Users\yordy\Documents\dev\bootcamp\.net\OrdenaFotos\photo_library";

			System.IO.Directory.CreateDirectory(photoLibraryDirectory);
			Handle(inboxDirectory, photoLibraryDirectory);
		}

		static void Handle(string inboxDirectory, string photoLibraryDirectory)
		{
			var imageFiles = System.IO.Directory.GetFiles(inboxDirectory, "*.jpg");

			foreach (var imagePath in imageFiles)
			{
				DateTime? dateTaken = ExempleExif.ObtenirDataExif(imagePath);

				if (dateTaken != null)
				{
					Console.WriteLine($"File: '{Path.GetFileName(imagePath)}'.");

					string year = dateTaken.Value.Year.ToString();
					string month = dateTaken.Value.Month.ToString("D2");

					string destinationDirectory = Path.Combine(photoLibraryDirectory, year, month);
					System.IO.Directory.CreateDirectory(destinationDirectory);

					string destinationPath = Path.Combine(destinationDirectory, Path.GetFileName(imagePath));
					if (!File.Exists(destinationPath))
					{
						File.Copy(imagePath, destinationPath);
					}
				}
			}
		}
	}
}