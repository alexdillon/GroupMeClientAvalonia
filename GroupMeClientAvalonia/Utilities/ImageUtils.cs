using Avalonia.Media.Imaging;
using System.IO;

namespace GroupMeClientAvalonia.Utilities
{
    /// <summary>
    /// Helper class to provide common Wpf image operations.
    /// </summary>
    public class ImageUtils
    {
        /// <summary>
        /// Converts a <see cref="BitmapSource"/> to encoded raw image bytes.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <returns>A png encoded array of raw bytes.</returns>
        public static byte[] BitmapSourceToBytes(IBitmap image)
        {
            using (var stream = new MemoryStream())
            {
                image.Save(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converts raw image data into an <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="image">The raw image data.</param>
        /// <returns>A Wpf <see cref="ImageSource"/>.</returns>
        public static IBitmap BytesToImageSource(byte[] image)
        {
            using (var ms = new MemoryStream(image))
            {
                return new Bitmap(ms);
            }
        }

        /// <summary>
        /// Converts raw image data into an <see cref="ImageSource"/>.
        /// </summary>
        /// <param name="image">The raw image data.</param>
        /// <param name="maxWidth">The maximum image width.</param>
        /// <param name="maxHeight">The maximum image height.</param>
        /// <returns>A Wpf <see cref="ImageSource"/>.</returns>
        public static IBitmap BytesToImageSource(byte[] image, int maxWidth, int maxHeight)
        {
            //TODO
            return BytesToImageSource(image);

            //using (var ms = new MemoryStream(image))
            //{
            //    var bitmapImage = new BitmapImage();
            //    bitmapImage.BeginInit();
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //    bitmapImage.StreamSource = ms;

            //    if (maxWidth > maxHeight)
            //    {
            //        bitmapImage.DecodePixelWidth = maxWidth;
            //    }
            //    else
            //    {
            //        bitmapImage.DecodePixelHeight = maxHeight;
            //    }

            //    bitmapImage.EndInit();
            //    bitmapImage.Freeze();

            //    return bitmapImage;
            //}
        }
    }
}
