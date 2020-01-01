using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Consumes class
    /// </summary>
    public sealed class Consumes : INotifyPropertyChanged
    {
        private string _voiceTime;
        public string VoiceTime
        {
            get { return _voiceTime; }
            set { Set(ref _voiceTime, value); }
        }

        private string _voiceExtra;
        public string VoiceExtra
        {
            get { return _voiceExtra; }
            set { Set(ref _voiceExtra, value); }
        }

        private string _SMSCount;
        public string SMSCount
        {
            get { return _SMSCount; }
            set { Set(ref _SMSCount, value); }
        }

        private string _SMSExtra;
        public string SMSExtra
        {
            get { return _SMSExtra; }
            set { Set(ref _SMSExtra, value); }
        }

        private string _dataUsed;
        public string DataUsed
        {
            get { return _dataUsed; }
            set { Set(ref _dataUsed, value); }
        }

        private string _dataExtra;
        public string DataExtra
        {
            get { return _dataExtra; }
            set { Set(ref _dataExtra, value); }
        }

        private string _MMSCount;
        public string MMSCount
        {
            get { return _MMSCount; }
            set { Set(ref _MMSCount, value); }
        }

        private string _MMSExtra;
        public string MMSExtra
        {
            get { return _MMSExtra; }
            set { Set(ref _MMSExtra, value); }
        }

        /// <summary>
        /// Parameterless constructor of Consumes class.
        /// </summary>
        public Consumes()
        {
            //
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
