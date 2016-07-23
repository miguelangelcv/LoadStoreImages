using LoadStoreImages.Models;
using LoadStoreImages.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;

namespace LoadStoreImages.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region Variables
        ImageStorage imgStorage;
        private string url;
        private string internetImage;
        private ObservableCollection<string> files;
        private BitmapImage storedImage;
        private string selectedImage;
        #endregion


        #region Commands
        private Lazy<DelegateCommand<string>> downloadButtonCommand;
        #endregion


        #region Properties
        public string Url
        {
            get { return url; }
            set
            {
                url = value;
                RaisePropertyChanged();
            }
        }

        public string InternetImage
        {
            get { return internetImage; }
            set
            {
                internetImage = value;
                RaisePropertyChanged();
            }
        }
        
        public ObservableCollection<string> Files
        {
            get { return files; }
            set
            {
                files = value;
                RaisePropertyChanged();
            }
        }
        
        public BitmapImage StoredImage
        {
            get { return storedImage; }
            set
            {
                storedImage = value;
                RaisePropertyChanged();
            }
        }       

        public string SelectedImage
        {
            get { return selectedImage; }
            set
            {
                selectedImage = value;
                LoadImage(selectedImage);
                RaisePropertyChanged();
            }
        }
        #endregion

        public ICommand DownloadButtonCommand
        {
            get { return downloadButtonCommand.Value; }
        }

        private async void DownloadButtonCommandExecute(string url)
        {
            MessageDialog msgDialog;
            if (CheckUriFormat(url))
            {
                bool isImage = await IsImageUrl(url);
                if (isImage)
                {
                    DownloadImage(url);
                }
                else
                {
                    msgDialog = new MessageDialog("La URL introducida no corresponde a la de una imagen");
                    await msgDialog.ShowAsync();
                }
            }
            else
            {
                msgDialog = new MessageDialog("Formato de la URL no válido");
                await msgDialog.ShowAsync();
            }
        }

        public MainViewModel()
        {
            imgStorage = new ImageStorage();
            files = new ObservableCollection<string>();
            downloadButtonCommand = new Lazy<DelegateCommand<string>>(
               () =>
               new DelegateCommand<string>(DownloadButtonCommandExecute));
            GetFiles(Windows.Storage.ApplicationData.Current.LocalFolder);
        }

        private async void LoadImage(string selectedImage)
        {
            if (selectedImage != null)
                StoredImage = await imgStorage.LoadImageFromStorage(selectedImage);
        }       

        private async void DownloadImage(string url)
        {
            InternetImage = url;
            await imgStorage.StoreImageFromURL(url);
            StoredImage = await imgStorage.LoadImageFromStorage(Path.GetFileName(url));
            MessageDialog msgDialog = new MessageDialog("Download Completed");
            await msgDialog.ShowAsync();
            GetFiles(Windows.Storage.ApplicationData.Current.LocalFolder);
        }

        private async void GetFiles(StorageFolder folder)
        {
            StorageFolder fold = folder;
            var items = await fold.GetItemsAsync();
            List<string> listFiles = new List<string>();

            foreach (var item in items)
            {
                listFiles.Add(item.Name);
            }

            Files = new ObservableCollection<string>(listFiles);
        }

        public bool CheckUriFormat(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        public async Task<bool> IsImageUrl(string URL)
        {
            var req = (HttpWebRequest)HttpWebRequest.Create(URL);
            req.Method = "HEAD";
            using (var resp = await req.GetResponseAsync())
            {
                return resp.ContentType.ToLower().StartsWith("image/");
            }
        }
    }
}