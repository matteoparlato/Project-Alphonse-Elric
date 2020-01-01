using System;

namespace Project_Alphonse_Elric.Core.Models
{
    /// <summary>
    /// Message class
    /// </summary>
    public sealed class Message
    {
        public string ID { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string DateTime { get; set; } = string.Empty;
        public Uri Source { get; set; }
        public string MIME { get; set; } = string.Empty;

        /// <summary>
        /// Constructor which initializes a Message object with passed information.
        /// </summary>
        /// <param name="sender">The sender of the message</param>
        /// <param name="dateTime">The date and the time of the message</param>
        /// <param name="id">The ID of the message</param>
        /// <param name="source">The source of the message</param>
        /// <param name="mime">The MIME type of the message</param>
        public Message(string sender, string dateTime, string id, string source, string mime)
        {
            Sender = sender;
            DateTime = dateTime;
            ID = id;
            Source = new Uri("https://www.iliad.it" + source);
            MIME = mime;
        }
    }
}
