namespace Vaccination.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        //IArtistRepository Artists { get; }
        //ICityRepository Cities { get; }
        //IConcertRepository Concerts { get; }
        //ICountryRepository Countries { get; }
        //IRoomRepository Rooms { get; }
        //IVenueRepository Venues { get; }
        //IConcertArtistRepository ConcertArtists { get; }
        //IConcertGoerRepository ConcertGoers { get; }

        Task SaveAsync();
    }
}