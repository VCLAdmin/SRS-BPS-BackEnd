namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="UnauthorizedException" />.
    /// </summary>
    public class UnauthorizedException : CustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public UnauthorizedException(string message)
        {
            UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        public UnauthorizedException()
        {
        }
    }
}
