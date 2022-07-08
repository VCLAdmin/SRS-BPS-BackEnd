namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="DocumentConflictException" />.
    /// </summary>
    public class DocumentConflictException : CustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentConflictException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public DocumentConflictException(string message)
        {
            this.UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentConflictException"/> class.
        /// </summary>
        public DocumentConflictException()
        {
        }
    }
}
