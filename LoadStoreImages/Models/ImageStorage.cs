using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace LoadStoreImages.Models
{
    public class ImageStorage
    {
        public async Task<BitmapImage> LoadImageFromStorage(string fileName)
        {
            byte[] bytes = null;

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Get the file.
            StorageFile file = await local.GetFileAsync(fileName);

            // Read the data.
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            bytes = buffer.ToArray();

            BitmapImage bImage = new BitmapImage();
            bImage = await ByteArrayToImage(bytes);
            return bImage;
        }

        public async Task<BitmapImage> LoadImageFromStorage(string fileName, string folder)
        {
            byte[] bytes = null;

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Get the DataFolder folder.
            var dataFolder = await local.GetFolderAsync(folder);

            // Get the file.
            StorageFile file = await dataFolder.GetFileAsync(fileName);

            // Read the data.
            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            bytes = buffer.ToArray();

            BitmapImage bImage = new BitmapImage();
            bImage = await ByteArrayToImage(bytes);
            return bImage;
        }

        private async Task<BitmapImage> ByteArrayToImage(byte[] byteArray)
        {
            BitmapImage image = new BitmapImage();
            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(byteArray);
                    await writer.StoreAsync();
                }
                await image.SetSourceAsync(stream);
                return image;
            }
        }

        public async Task StoreImageFromURL(string url)
        {
            string FileName = Path.GetFileName(url);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage message = await httpClient.GetAsync(url);

            // Get the text data from the textbox. 
            byte[] fileBytes = await message.Content.ReadAsByteArrayAsync();

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Current folder to store
            StorageFolder myfolder = ApplicationData.Current.LocalFolder;

            // Create a new file named DataFile.txt.
            StorageFile file = await myfolder.CreateFileAsync(Path.GetFileName(url),
            CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }
        }

        public async void StoreImageFromURL(string url, string folder)
        {
            string FileName = Path.GetFileName(url);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage message = await httpClient.GetAsync(url);

            // Get the text data from the textbox. 
            byte[] fileBytes = await message.Content.ReadAsByteArrayAsync();

            // Get the local folder.
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;

            // Create a new folder name DataFolder.
            StorageFolder dataFolder = await local.CreateFolderAsync(folder,
                CreationCollisionOption.OpenIfExists);

            // Current folder to store
            StorageFolder myfolder = ApplicationData.Current.LocalFolder;

            // Create a new file named DataFile.txt.
            StorageFile file = await myfolder.CreateFileAsync(Path.GetFileName(url),
            CreationCollisionOption.ReplaceExisting);

            // Write the data from the textbox.
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }
        }
    }
}
