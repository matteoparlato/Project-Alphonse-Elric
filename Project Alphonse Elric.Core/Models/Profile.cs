namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Profile class
    /// </summary>
    public sealed class Profile
    {
        public Consumes Local { get; set; } = new Consumes();
        public Consumes Roaming { get; set; } = new Consumes();
        public string Name { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string RemainingCredit { get; set; } = string.Empty;
        public string NextRenewal { get; set; } = string.Empty;
        public Options ActiveOptions { get; set; } = new Options();
        public Services ActiveServices { get; set; } = new Services();
        public Voicemail Voicemail { get; set; } = new Voicemail();

        /// <summary>
        /// Parameterless constructor of Profile class.
        /// </summary>
        public Profile()
        {
            //
        }
    }
}