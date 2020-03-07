using System.Collections.ObjectModel;

namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Voicemail class
    /// </summary>
    public sealed class Voicemail : BindableBase
    {
        private bool _showCallerID;
        public bool ShowCallerID
        {
            get => _showCallerID;
            set => SetProperty(ref _showCallerID, value);
        }

        private bool _showTimeDate;
        public bool ShowTimeDate
        {
            get => _showTimeDate;
            set => SetProperty(ref _showTimeDate, value);
        }

        private bool _protectVoicemail;
        public bool ProtectVoicemail
        {
            get => _protectVoicemail;
            set => SetProperty(ref _protectVoicemail, value);
        }

        private bool _personalizedAdvert;
        public bool PersonalizedAdvert
        {
            get => _personalizedAdvert;
            set => SetProperty(ref _personalizedAdvert, value);
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
    }
}
