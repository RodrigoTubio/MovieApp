using MoviesApp.Models;

namespace MoviesApp.Interfaces
{
    public interface ISearchMovieService
    {
        public HttpResponseMessage ApiCall(string name);
        public MovieModel Result(HttpResponseMessage result);
    }
}
