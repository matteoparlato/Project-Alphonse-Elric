namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Options class
    /// </summary>
    public sealed class Options : BindableBase
    {
        public bool LTE { get; private set; } = true;

        private bool _showLastThreeNumbers;
        public bool ShowLastThreeNumbers
        {
            get => _showLastThreeNumbers;
            set => SetProperty(ref _showLastThreeNumbers, value);
        }

        private bool _paidNumbers;
        public bool PaidNumbers
        {
            get => _paidNumbers;
            set => SetProperty(ref _paidNumbers, value);
        }

        private bool _paidBankNumbers;
        public bool PaidBankNumbers
        {
            get => _paidBankNumbers;
            set => SetProperty(ref _paidBankNumbers, value);
        }

        private bool _unlockLocal;
        public bool UnlockLocal
        {
            get => _unlockLocal;
            set => SetProperty(ref _unlockLocal, value);
        }

        private bool _unlockItaly;
        public bool UnlockItaly
        {
            get => _unlockItaly;
            set => SetProperty(ref _unlockItaly, value);
        }

        private bool _unlockRoaming;
        public bool UnlockRoaming
        {
            get => _unlockRoaming;
            set => SetProperty(ref _unlockRoaming, value);
        }

        private bool _marineRoaming;
        public bool MarineRoaming
        {
            get => _marineRoaming;
            set => SetProperty(ref _marineRoaming, value);
        }

        private bool _marketingAgreement;
        public bool MarketingAgreement
        {
            get => _marketingAgreement;
            set => SetProperty(ref _marketingAgreement, value);
        }

        /// <summary>
        /// Parameterless constructor of Options class.
        /// </summary>
        public Options()
        {
            //
        }
    }
}
