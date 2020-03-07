namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Options class
    /// </summary>
    public sealed class Options : BindableBase
    {
        public bool LTE { get; private set; } = true;

        private bool _publishPhoneNumber;
        public bool PublishPhoneNumber
        {
            get => _publishPhoneNumber;
            set => SetProperty(ref _publishPhoneNumber, value);
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
