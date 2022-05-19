namespace VCLWebAPI.Exceptions
{
    /// <summary>
    /// Defines the <see cref="NotFoundException" />.
    /// </summary>
    public class NotFoundException : CustomException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message<see cref="string"/>.</param>
        public NotFoundException(string message)
        {
            this.UserMessage = message;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        public NotFoundException()
        {
        }
    }
}
