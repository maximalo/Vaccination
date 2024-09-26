using AutoMapper;
using Vaccination.Domain.Shared;

namespace Vaccination.Application.Mapper
{
    /// <summary>
    /// Converts a <see cref="PagedList{TSource}"/> to a <see cref="PagedList{TDestination}"/> using AutoMapper.
    /// </summary>
    /// <typeparam name="TSource">The type of the source items.</typeparam>
    /// <typeparam name="TDestination">The type of the destination items.</typeparam>
    public class PagedListConverter<TSource, TDestination> : ITypeConverter<PagedList<TSource>, PagedList<TDestination>>
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedListConverter{TSource, TDestination}"/> class.
        /// </summary>
        /// <param name="mapper">The AutoMapper instance.</param>
        public PagedListConverter(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Converts a <see cref="PagedList{TSource}"/> to a <see cref="PagedList{TDestination}"/>.
        /// </summary>
        /// <param name="source">The source <see cref="PagedList{TSource}"/>.</param>
        /// <param name="destination">The destination <see cref="PagedList{TDestination}"/>.</param>
        /// <param name="context">The resolution context.</param>
        /// <returns>The converted <see cref="PagedList{TDestination}"/>.</returns>
        public PagedList<TDestination> Convert(PagedList<TSource> source, PagedList<TDestination> destination, ResolutionContext context)
        {
            var mappedItems = _mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source);
            return new PagedList<TDestination>(mappedItems.ToList(), source.TotalCount, source.CurrentPage, source.PageSize);
        }
    }
}