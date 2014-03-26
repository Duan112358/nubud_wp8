using Nubud.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Nubud.Converters
{
    /// <summary>
    /// Caches the image that gets downloaded as part of Image control Source property.
    /// </summary>
    public class LocalCache : IValueConverter
    {
        private static IsolatedStorageFile _storage;
        private const string imageStorageFolder = "TempImages";
        private const string articleStorageFolder = "ArtileContents";
        private const string EMPTY_PHONT = "/Nubud;component/Assets/empty.png";
        private static Dictionary<string, BitmapImage> ImageCache =
            new Dictionary<string, BitmapImage>();

        public LocalCache()
        {
            try
            {
                if (_storage == null)
                {
                    _storage = IsolatedStorageFile.GetUserStoreForApplication();
                }
            }
            catch (IsolatedStorageException)
            {
                //do nothing...
            }
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value as string;
            if (String.IsNullOrEmpty(path)) return null;

            if (ImageCache.ContainsKey(path))
            {
                return ImageCache[path];
            }

            Uri imageFileUri = new Uri(path);
            if (imageFileUri.Scheme == "http" || imageFileUri.Scheme == "https")
            {
                if (!Utils.IsNetworkAvailable())
                {
                    if (_storage.FileExists(GetFileNameInIsolatedStorage(imageFileUri)))
                    {
                        var image = ExtractFromLocalStorage(imageFileUri);
                        ImageCache.Add(path, image);
                        return image;
                    }
                    else
                    {
                        var image = LoadDefaultIfPassed(imageFileUri, (parameter ?? EMPTY_PHONT).ToString());
                        ImageCache.Add(path, image);
                        return image;
                    }
                }
                else
                {
                    var image = DownloadFromWeb(imageFileUri);
                    ImageCache.Add(path, image);
                    return image;
                }
            }
            else
            {
                BitmapImage bm = new BitmapImage(imageFileUri);
                return bm;
            }
        }

        public static BitmapImage LoadImage(string path)
        {
            if (String.IsNullOrEmpty(path)) return null;
            Uri imageFileUri = new Uri(path, UriKind.RelativeOrAbsolute);
            if (imageFileUri.Scheme == "http" || imageFileUri.Scheme == "https")
            {
                if (!Utils.IsNetworkAvailable())
                {
                    if (_storage.FileExists(GetFileNameInIsolatedStorage(imageFileUri)))
                    {
                        return ExtractFromLocalStorage(imageFileUri);
                    }
                    else
                    {
                        return LoadDefaultIfPassed(imageFileUri, EMPTY_PHONT);
                    }
                }
                else
                {
                    return DownloadFromWeb(imageFileUri);
                }
            }
            else
            {
                BitmapImage bm = new BitmapImage(imageFileUri);
                return bm;
            }
        }

        private static BitmapImage LoadDefaultIfPassed(Uri imageFileUri, string defaultImagePath)
        {
            string defaultImageUri = (defaultImagePath ?? EMPTY_PHONT).ToString();
            if (!string.IsNullOrEmpty(defaultImageUri))
            {
                BitmapImage bm = new BitmapImage(new Uri(defaultImageUri, UriKind.RelativeOrAbsolute));         //Load default Image
                return bm;
            }
            else
            {
                BitmapImage bm = new BitmapImage(imageFileUri);
                return bm;
            }
        }

        private static BitmapImage DownloadFromWeb(Uri imageFileUri)
        {
            WebClient m_webClient = new WebClient();                                //Load from internet
            BitmapImage bm = new BitmapImage();

            m_webClient.OpenReadCompleted += (o, e) =>
            {
                if (e.Error != null || e.Cancelled) return;
                WriteToIsolatedStorage(IsolatedStorageFile.GetUserStoreForApplication(), e.Result, GetFileNameInIsolatedStorage(imageFileUri));
                bm.SetSource(e.Result);
                e.Result.Close();
            };
            m_webClient.OpenReadAsync(imageFileUri);
            return bm;
        }

        private static BitmapImage ExtractFromLocalStorage(Uri imageFileUri)
        {
            string isolatedStoragePath = GetFileNameInIsolatedStorage(imageFileUri);       //Load from local storage
            using (var sourceFile = _storage.OpenFile(isolatedStoragePath, FileMode.Open, FileAccess.Read))
            {
                BitmapImage bm = new BitmapImage();
                bm.SetSource(sourceFile);
                return bm;
            }
        }

        public static void ClearLocalStorage()
        {
            var filePaths = App.ArticlesViewModel.Articles.Select(a => a.Image);
            foreach (var path in filePaths)
            {
                var uri = GetFileNameInIsolatedStorage(new Uri(path, UriKind.RelativeOrAbsolute));
                if (_storage.FileExists(uri))
                {
                    _storage.DeleteFile(uri);
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static void WriteToIsolatedStorage(IsolatedStorageFile storage, System.IO.Stream inputStream, string fileName)
        {
            IsolatedStorageFileStream outputStream = null;
            try
            {
                if (!storage.DirectoryExists(imageStorageFolder))
                {
                    storage.CreateDirectory(imageStorageFolder);
                }
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
                outputStream = storage.CreateFile(fileName);
                byte[] buffer = new byte[32768];
                int read;
                while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, read);
                }
                outputStream.Close();
            }
            catch
            {
                //We cannot do anything here.
                if (outputStream != null) outputStream.Close();
            }
        }

        /// <summary>
        /// Gets the file name in isolated storage for the Uri specified. This name should be used to search in the isolated storage.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static string GetFileNameInIsolatedStorage(Uri uri)
        {
            return imageStorageFolder + "\\" + uri.AbsoluteUri.GetHashCode() + ".img";
        }

    }
}
