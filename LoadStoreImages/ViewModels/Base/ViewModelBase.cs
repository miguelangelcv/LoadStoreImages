using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LoadStoreImages.ViewModels.Base
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase() { }       
                        
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateAll()
        {
            RaisePropertyChanged(string.Empty);
        }
    }
}
