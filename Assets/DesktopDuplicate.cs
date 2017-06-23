using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;

namespace ScreenshotCapturer
{
	class Program
	{
		static void Main(string[] args)
		{
			// Usage:
			// ScreenshotCapturer.exe [-prefix <filePathStemToSaveTo>] [-extension <imageFileExtension>]
			// [-bounds <captureBoundsX> <captureBoundsY> <captureBoundsWidth> <captureBoundsHeight>]
			// [-count <numberOfScreenshotsToCapture>] [-frequency <screenshotsPerSecond>]

			DateTime now = DateTime.Now;
			Console.WriteLine("Program begins: " + now);

			const string dateTimeFormat = "yyyyMMdd-HHmmss-fff";

			string filePathStemToSaveTo = "Screenshot-" + now.ToString(dateTimeFormat);
			string imageFileExtension = ".png";
			Rectangle? captureBoundsNullable = null;
			int numberOfScreenshotsToCapture = 1;
			int millisecondsPerScreenshot = 0;

			try
			{
				for (var i = 0; i < args.Length; i++)
				{
					var argument = args[i];
					if (argument == "-prefix")
					{
						filePathStemToSaveTo = args[i + 1];
						i++;		
					}
					else if (argument == "-extension")
					{
						imageFileExtension = args[i + 1];
						i++;
					}
					else if (argument == "-bounds")
					{
						captureBoundsNullable = new Rectangle
						(
							Int32.Parse(args[i + 1]), Int32.Parse(args[i + 2]),
							Int32.Parse(args[i + 3]), Int32.Parse(args[i + 4])
						);
						i += 4;
					}
					else if (argument == "-count")
					{
						numberOfScreenshotsToCapture = Int32.Parse(args[i + 1]);
						i++;
					}
					else if (argument == "-frequency")
					{
						int screenshotsPerSecond = Int32.Parse(args[i + 1]);
						millisecondsPerScreenshot = 1000 / screenshotsPerSecond;
						i++;
					}
					else
					{						
						throw new Exception( "Unrecognized command-line switch:" + argument);
					}
				}
			}
			catch (Exception ex)
			{
				string errorMessage = "Error parsing command-line arguments: " + ex.Message;
				Console.WriteLine(errorMessage);
			}
				
			ScreenshotCapturer screenshotCapturer = new ScreenshotCapturer()
			{
					FilePathStemToSaveTo = filePathStemToSaveTo,
					ImageFileExtension = imageFileExtension,
					CaptureBoundsNullable = captureBoundsNullable,
					NumberOfScreenshotsToCapture = numberOfScreenshotsToCapture,
					MillisecondsPerScreenshot = millisecondsPerScreenshot,
			};
			
			screenshotCapturer.start();

			Console.WriteLine("Program ends: " + DateTime.Now);
		}
	}

	public class ScreenshotCapturer
	{
		public string FilePathStemToSaveTo;
		public string ImageFileExtension;
		public Rectangle? CaptureBoundsNullable;
		public int NumberOfScreenshotsToCapture;
		public int MillisecondsPerScreenshot;
	
		public void start()
		{
			Console.WriteLine("-prefix=" + this.FilePathStemToSaveTo);
			Console.WriteLine("-extension=" + this.ImageFileExtension);
			Console.WriteLine("-bounds=" + this.CaptureBoundsNullable);
			Console.WriteLine("-count=" + this.NumberOfScreenshotsToCapture);
			Console.WriteLine("-frequency=" + (1000 / this.MillisecondsPerScreenshot));

			if (this.NumberOfScreenshotsToCapture == 1)
			{
				this.copyScreenToImageFile(0);
			}
			else
			{
				for (var i = 0; i < this.NumberOfScreenshotsToCapture; i++)
				{
					this.copyScreenToImageFile(i);
					Thread.Sleep(this.MillisecondsPerScreenshot);
				}
			}	
		}

		public void copyScreenToImageFile(int? imageIndex)
		{
			// Adapted from code found at the URL
			// https://stackoverflow.com/questions/5049122/capture-the-screen-shot-using-net

			Rectangle captureBounds;

			if (this.CaptureBoundsNullable == null)
			{
				captureBounds = Screen.PrimaryScreen.Bounds;
			}
			else
			{
				captureBounds = this.CaptureBoundsNullable.Value;
			}	

			Bitmap bitmapCaptured = new Bitmap
			(
				captureBounds.Width, 
				captureBounds.Height
			);
			
			Graphics graphics = Graphics.FromImage(bitmapCaptured);
			
			graphics.CopyFromScreen
			(
				captureBounds.X, captureBounds.Y, // source
				0, 0, // destination
				bitmapCaptured.Size 
				// CopyPixelOperation.SourceCopy
			);

			string filePathToSaveTo = 
				this.FilePathStemToSaveTo 
				+ (imageIndex == null ? "" : imageIndex.Value.ToString())
				+ this.ImageFileExtension;

			Console.WriteLine("Saving " + filePathToSaveTo + "...");

			try
			{
				bitmapCaptured.Save(filePathToSaveTo);
			}
			catch (Exception)
			{
				string errorMessage = 
					"Error attemping to save file.  Ensure directory exists, and permissions are adequate.";
				Console.WriteLine(errorMessage);
			}
		}
	}
}
