using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    /// <summary>
    /// Voicemail class
    /// </summary>
    public sealed class Voicemail : INotifyPropertyChanged
    {
        private bool _showCallerID;
        public bool ShowCallerID
        {
            get { return _showCallerID; }
            set { Set(ref _showCallerID, value); }
        }

        private bool _showTimeDate;
        public bool ShowTimeDate
        {
            get { return _showTimeDate; }
            set { Set(ref _showTimeDate, value); }
        }

        private bool _protectVoicemail;
        public bool ProtectVoicemail
        {
            get { return _protectVoicemail; }
            set { Set(ref _protectVoicemail, value); }
        }

        private bool _personalizedAdvert;
        public bool PersonalizedAdvert
        {
            get { return _personalizedAdvert; }
            set { Set(ref _personalizedAdvert, value); }
        }

        public readonly ObservableCollection<Message> MessagesList = new ObservableCollection<Message>();

        public readonly ObservableCollection<VoicemailNotification> VoicemailNotificationList = new ObservableCollection<VoicemailNotification>();

        /// <summary>
        /// Parameterless constructor of Voicemail class.
        /// </summary>
        public Voicemail()
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
