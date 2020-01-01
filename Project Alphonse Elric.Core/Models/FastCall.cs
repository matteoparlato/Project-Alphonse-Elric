namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// FastCall class
    /// </summary>
    public sealed class FastCall
    {
        private string _name = string.Empty;
        public string Name
        {
            get { return _name.ToUpper(); }
            set { _name = value; }
        }
        public string Target { get; set; } = string.Empty;
        public string ShortTarget { get; set; } = string.Empty;
        public string DeleteLink { get; set; } = string.Empty;

        /// <summary>
        /// Constructor which initializes a FastCall object with passed information.
        /// </summary>
        /// <param name="name">The name of the fast call</param>
        /// <param name="target">The target number of the fast call</param>
        /// <param name="shortTarget">The abbreviation of the target number</param>
        /// <param name="deleteLink">The delete link of the fast call</param>
        public FastCall(string name, string target, string shortTarget, string deleteLink)
        {
            Name = name;
            Target = target;
            ShortTarget = shortTarget;
            DeleteLink = deleteLink;
        }
    }
}
