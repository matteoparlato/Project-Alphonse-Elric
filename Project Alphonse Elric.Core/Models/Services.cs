using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Services class
    /// </summary>
    public sealed class Services : INotifyPropertyChanged
    {
        private bool _blockUnknown;
        public bool BlockUnknown
        {
            get { return _blockUnknown; }
            set
            {
                if (value == false)
                {
                    RedirectToVoicemailUnknown = false;
                }
                Set(ref _blockUnknown, value);
            }
        }

        private bool _redirectToVoicemailUnknown;
        public bool RedirectToVoicemailUnknown
        {
            get { return _redirectToVoicemailUnknown; }
            set { Set(ref _redirectToVoicemailUnknown, value); }
        }

        private bool _transferToVoicemail;
        public bool TransferToVoicemail
        {
            get { return _transferToVoicemail; }
            set { Set(ref _transferToVoicemail, value); }
        }

        private bool _transferProtection;
        public bool TransferProtection
        {
            get { return _transferProtection; }
            set { Set(ref _transferProtection, value); }
        }

        private bool _userNotAvailable;
        public bool UserNotAvailable
        {
            get { return _userNotAvailable; }
            set { Set(ref _userNotAvailable, value); }
        }

        private bool _fastCalls;
        public bool FastCalls
        {
            get { return _fastCalls; }
            set { Set(ref _fastCalls, value); }
        }

        public readonly ObservableCollection<FastCall> FastCallList = new ObservableCollection<FastCall>();

        private bool _filter;
        public bool Filter
        {
            get { return _filter; }
            set { Set(ref _filter, value); }
        }

        /// <summary>
        /// Parameterless constructor of Services class.
        /// </summary>
        public Services()
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
