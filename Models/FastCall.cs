namespace Models
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
        public string Short { get; set; } = string.Empty;
        public string DeleteLink { get; set; } = string.Empty;

        /// <summary>
        /// Constructor which initializes a FastCall object with passed information.
        /// </summary>
        /// <param name="name">The name of the shortening</param>
        /// <param name="target">The target of the shortening</param>
        /// <param name="short">The shortening</param>
        /// <param name="deleteLink">The delete link of the shortening</param>
        public FastCall(string name, string target, string @short, string deleteLink)
        {
            Name = name;
            Target = target;
            Short = @short;
            DeleteLink = deleteLink;
        }
    }
}
