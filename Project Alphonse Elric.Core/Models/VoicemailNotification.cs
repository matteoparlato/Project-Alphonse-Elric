namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// VoicemailNotification class
    /// </summary>
    public sealed class VoicemailNotification : BindableBase
    {
        private string _mode = string.Empty;
        public string Mode
        {
            get => _mode.ToUpper();
            set => _mode = value;
        }

        public string Mail { get; set; } = string.Empty;

        /// <summary>
        /// Constructor which initializes a VoicemailNotification object with passed information.
        /// </summary>
        /// <param name="mode">The notification mode</param>
        /// <param name="mail">The mail where to send the notification</param>
        public VoicemailNotification(string mode, string mail)
        {
            Mode = mode;
            Mail = mail;
        }
    }
}
