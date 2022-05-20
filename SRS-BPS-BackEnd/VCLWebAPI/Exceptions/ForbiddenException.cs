namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="ForbiddenException" />.
    /// </summary>
    public class ForbiddenException : CustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public ForbiddenException(string message)
        {
            this.UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        public ForbiddenException()
        {
        }
    }
}
