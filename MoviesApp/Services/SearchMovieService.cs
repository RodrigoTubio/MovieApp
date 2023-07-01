using MoviesApp.Entities;
using MoviesApp.Interfaces;
using MoviesApp.Models;
using Serilog;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;

namespace MoviesApp.Services
{
    public class SearchMovieService : ISearchMovieService
    {
        private readonly ISearchSimilarMoviesService _searchSimilarMoviesService;
        private readonly IConfiguration _configuration;

        public SearchMovieService(IConfiguration configuration, ISearchSimilarMoviesService searchSimilarMoviesService)
        {
            _searchSimilarMoviesService = searchSimilarMoviesService;
            _configuration = configuration;
        }

        public HttpResponseMessage ApiCall(string name)
        {
            var token = _configuration.GetSection("Settings")["token"];
            HttpResponseMessage result = null;
            try
            {
                using (var client = new HttpClient())
                {
                    var uriBuilder = new UriBuilder("https://api.themoviedb.org/3/search/movie");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    var parameters = HttpUtility.ParseQueryString(string.Empty);
                    parameters["query"] = name;
                    parameters["language"] = "es-ES";
                    uriBuilder.Query = parameters.ToString();
                    Uri finalUrl = uriBuilder.Uri;
                    Log.Information($"START Request -> URL: {finalUrl}");
                    result = client.GetAsync(finalUrl).Result;
                    Log.Information($"END Request -> StatusCode: {result.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Log.Information($"ERROR -> Excepction: {ex}");
            }
            return result;
        }
  
        public MovieModel Result(HttpResponseMessage result)
        {
            string moviesResult = result.Content.ReadAsStringAsync().Result;

            try
            {
                MoviesList? movies = JsonSerializer.Deserialize<MoviesList>(moviesResult);

                string resultSimilarMovies = _searchSimilarMoviesService.GetSimilarMovies(movies.results[0].id);
                
                MoviesList? moviesSimilar = JsonSerializer.Deserialize<MoviesList>(resultSimilarMovies);

                MovieModel movieModel = MapMovieSearch(movies.results, moviesSimilar.results);

                return movieModel;
            }
            catch (Exception ex)
            {
                Log.Information($"ERROR -> Exception: {ex}");
            };
            return null;
        }

        private static MovieModel MapMovieSearch(List<Movie> movies, List<Movie> moviesSimilar)
        {
            List<string> peliculas = new List<string>();
            if (moviesSimilar != null && moviesSimilar.Count > 0)
            {
                for (int i = 0; i < Math.Min(5, moviesSimilar.Count); i++)
                {
                    string date = moviesSimilar[i].release_date.Length > 4 ? moviesSimilar[i].release_date.Substring(0, 4) : "";
                    string pelicula = string.Format($"{moviesSimilar[i].title} ({date})");
                    peliculas.Add(pelicula);
                }
            }
            string concat = string.Join(", ", peliculas);
            
            MovieModel movieModel = new MovieModel()
            {
                Titulo = movies[0].title,
                Titulo_original = movies[0].original_title,
                Nota_media = movies[0].vote_average,
                Fecha_estreno = movies[0].release_date != "" ? DateTime.Parse(movies[0].release_date).ToString("dd-MM-yyyy") : "",
                Descripcion = movies[0].overview,
                Peliculas_misma_tematica = concat
            };

            return movieModel;
        }
    }
}