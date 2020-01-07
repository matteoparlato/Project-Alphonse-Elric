using System.Collections.ObjectModel;

namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Services class
    /// </summary>
    public sealed class Services : BindableBase
    {
        private bool _blockUnknown;
        public bool BlockUnknown
        {
            get => _blockUnknown;
            set
            {
                if (value == false)
                {
                    RedirectToVoicemailUnknown = false;
                }
                SetProperty(ref _blockUnknown, value);
            }
        }

        private bool _redirectToVoicemailUnknown;
        public bool RedirectToVoicemailUnknown
        {
            get => _redirectToVoicemailUnknown;
            set { SetProperty(ref _redirectToVoicemailUnknown, value); }
        }

        private bool _transferToVoicemail;
        public bool TransferToVoicemail
        {
            get => _transferToVoicemail;
            set { SetProperty(ref _transferToVoicemail, value); }
        }

        private bool _transferProtection;
        public bool TransferProtection
        {
            get => _transferProtection;
            set => SetProperty(ref _transferProtection, value);
        }

        private bool _userNotAvailable;
        public bool UserNotAvailable
        {
            get => _userNotAvailable;
            set => SetProperty(ref _userNotAvailable, value);
        }

        private bool _fastCalls;
        public bool FastCalls
        {
            get => _fastCalls;
            set => SetProperty(ref _fastCalls, value);
        }

        public readonly ObservableCollection<FastCall> FastCallList = new ObservableCollection<FastCall>();

        private bool _filter;
        public bool Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        /// <summary>
        /// Parameterless constructor of Services class.
        /// </summary>
        public Services()
        {
            //
        }
    }
}
