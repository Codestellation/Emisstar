namespace Codestellation.Emisstar.Impl
{
    public class PublisherSettings
    {
        public static readonly PublisherSettings Default = new PublisherSettings();

        /// <summary>
        /// Throws exception if no handlers found for a nessage
        /// </summary>
        public bool IgnoreNoHandlers { get; set; }

        /// <summary>
        /// Publisher will throw exception if no dispatcher found for a message
        /// </summary>
        public bool IgnoreNoDispatcher { get; set; }

        /// <summary>
        /// Publisher will throw exception mulitiple dispatchers forund for a message
        /// </summary>
        public bool IgnoreMultipleDispatcher { get; set; }
    }
}