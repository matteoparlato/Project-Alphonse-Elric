using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Models
{
    /// <summary>
    /// Options class
    /// </summary>
    public sealed class Options : INotifyPropertyChanged
    {
        public bool LTE { get; private set; } = true;

        private bool _publishPhoneNumber;
        public bool PublishPhoneNumber
        {
            get { return _publishPhoneNumber; }
            set { Set(ref _publishPhoneNumber, value); }
        }

        private bool _paidNumbers;
        public bool PaidNumbers
        {
            get { return _paidNumbers; }
            set { Set(ref _paidNumbers, value); }
        }

        private bool _paidBankNumbers;
        public bool PaidBankNumbers
        {
            get { return _paidBankNumbers; }
            set { Set(ref _paidBankNumbers, value); }
        }

        private bool _unlockLocal;
        public bool UnlockLocal
        {
            get { return _unlockLocal; }
            set { Set(ref _unlockLocal, value); }
        }

        private bool _unlockItaly;
        public bool UnlockItaly
        {
            get { return _unlockItaly; }
            set { Set(ref _unlockItaly, value); }
        }

        private bool _unlockRoaming;
        public bool UnlockRoaming
        {
            get { return _unlockRoaming; }
            set { Set(ref _unlockRoaming, value); }
        }

        /// <summary>
        /// Parameterless constructor of Options class.
        /// </summary>
        public Options()
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
