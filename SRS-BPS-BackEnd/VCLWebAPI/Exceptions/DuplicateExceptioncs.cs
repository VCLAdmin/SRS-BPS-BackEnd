namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="DuplicateException" />.
    /// </summary>
    public class DuplicateException : CustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public DuplicateException(string message)
        {
            this.UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateException"/> class.
        /// </summary>
        public DuplicateException()
        {
        }
    }
}
