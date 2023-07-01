using MoviesApp.Interfaces;
using Serilog;
using System.Net.Http.Headers;
using System.Web;

namespace MoviesApp.Services
{
    public class SearchSimilarMoviesService : ISearchSimilarMoviesService
    {
        private readonly IConfiguration _configuration;
        public SearchSimilarMoviesService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetSimilarMovies(int id)
        {
            var token = _configuration.GetSection("Settings")["token"];
            HttpResponseMessage result = null;
            using (var client = new HttpClient())
            {
                var uriBuilder = new UriBuilder($"https://api.themoviedb.org/3/movie/{id}/similar");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var parameters = HttpUtility.ParseQueryString(string.Empty);
                parameters["language"] = "es-ES";
                uriBuilder.Query = parameters.ToString();
                Uri finalUrl = uriBuilder.Uri;

                try
                {
                    Log.Information($"START Request -> URL: {finalUrl}");
                    result = client.GetAsync(finalUrl).Result;
                    Log.Information($"END Request -> StatusCode: {result.StatusCode}");
                }
                catch (Exception ex)
                {
                    Log.Information($"ERROR -> Exception: {ex}");
                }

            }

            string resultMovies = result.Content.ReadAsStringAsync().Result;
            Log.Information($"MovieModel: {result}");

            return resultMovies;
        }
    }
}
