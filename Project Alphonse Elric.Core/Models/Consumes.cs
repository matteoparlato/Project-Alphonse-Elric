namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Consumes class
    /// </summary>
    public sealed class Consumes : BindableBase
    {
        private string _voiceTime;
        public string VoiceTime
        {
            get => _voiceTime;
            set => SetProperty(ref _voiceTime, value);
        }

        private string _voiceExtra;
        public string VoiceExtra
        {
            get => _voiceExtra;
            set => SetProperty(ref _voiceExtra, value);
        }

        private string _SMSCount;
        public string SMSCount
        {
            get => _SMSCount;
            set => SetProperty(ref _SMSCount, value);
        }

        private string _SMSExtra;
        public string SMSExtra
        {
            get => _SMSExtra;
            set => SetProperty(ref _SMSExtra, value);
        }

        private string _dataUsed;
        public string DataUsed
        {
            get => _dataUsed;
            set => SetProperty(ref _dataUsed, value);
        }

        private string _dataExtra;
        public string DataExtra
        {
            get => _dataExtra;
            set => SetProperty(ref _dataExtra, value);
        }

        private string _MMSCount;
        public string MMSCount
        {
            get => _MMSCount;
            set => SetProperty(ref _MMSCount, value);
        }

        private string _MMSExtra;
        public string MMSExtra
        {
            get => _MMSExtra;
            set => SetProperty(ref _MMSExtra, value);
        }

        /// <summary>
        /// Parameterless constructor of Consumes class.
        /// </summary>
        public Consumes()
        {
            //
        }
    }
}
