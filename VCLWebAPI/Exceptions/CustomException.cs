using System;

namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="CustomException" />.
    /// </summary>
    public class CustomException : Exception
    {
        /// <summary>
        /// Gets or sets the UserMessage.
        /// </summary>
        public string UserMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public CustomException(string message)
        {
            this.UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomException"/> class.
        /// </summary>
        public CustomException()
        {
        }
    }
}
