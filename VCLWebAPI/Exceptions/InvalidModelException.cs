namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="InvalidModelException" />.
    /// </summary>
    public class InvalidModelException : CustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidModelException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public InvalidModelException(string message)
        {
            this.UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidModelException"/> class.
        /// </summary>
        public InvalidModelException()
        {
        }
    }
}
