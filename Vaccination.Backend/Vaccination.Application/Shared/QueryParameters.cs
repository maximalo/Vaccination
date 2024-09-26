namespace Vaccination.Application.Shared
{
    /// <summary>
    /// Base class for query parameters used in pagination.
    /// </summary>
    public abstract class QueryParameters
    {
        private const int maxPageSize = 50;

        /// <summary>
        /// Gets or sets the page number.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        /// <value>
        /// The page size. The maximum page size is 50.
        /// </value>
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value > maxPageSize ? maxPageSize : value;
            }
        }
    }
}