using KinectDemo2.Custom.Helper.Settings;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KinectDemo2.Custom.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected int GetAdjustedInterval() => SettingsManager.Interval() * 1000;
    }
}
