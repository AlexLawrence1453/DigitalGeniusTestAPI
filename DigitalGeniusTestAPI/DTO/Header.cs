
namespace DigitalGeniusTestAPI.DTO
{
    /// <summary>
    /// Request header.
    /// </summary>
    public class Header
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class.
        /// </summary>
        /// <param name="headerDesc">Header descriptor.</param>
        /// <param name="headerVal">Header value.</param>
        public Header(string headerDesc, string headerVal)
        {
            this.HeaderDescriptor = headerDesc;
            this.HeaderValue = headerVal;
        }

        /// <summary>
        /// Gets or sets the header descriptor.
        /// </summary>
        public string HeaderDescriptor { get; set; }

        /// <summary>
        /// Gets or sets the header value.
        /// </summary>
        public string HeaderValue { get; set; }
    }
}
