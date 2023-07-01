using Microsoft.AspNetCore.Mvc;
using MoviesApp.Interfaces;
using MoviesApp.Models;
using Serilog;
using System.Text.Json;


namespace MoviesApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ISearchMovieService _searchResult;

        public MoviesController(ISearchMovieService searchResult)
        {
            _searchResult = searchResult;
        }

        /// <summary>
        /// Resource to get a movie by name and its similar movies.
        /// </summary>
        /// <param name="name">string param</param>
        /// <returns>MovieModel</returns>
        /// 
        [HttpGet("{name}")]
        [Produces("application/json")]
        public MovieModel Get(string name)
        {
            MovieModel? content = null;
            try
            {
                var result = _searchResult.ApiCall(name);
                content = _searchResult.Result(result);
                string jsonString = JsonSerializer.Serialize(content);
                Log.Information($"MovieModel: {jsonString}");
            }
            catch (Exception ex)
            {
                Log.Information($"ERROR -> Exception: {ex}");
            }

            return content;
        }
    }
}