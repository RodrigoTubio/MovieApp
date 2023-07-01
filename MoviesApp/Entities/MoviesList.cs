namespace MoviesApp.Entities
{
    public class MoviesList
    {
        public int page { get; set; }

        public List<Movie> results { get; set; }

        public int total_pages { get; set; }

        public int total_results { get; set; }

        public MoviesList()
        {
            results = new List<Movie>();
        }
    }
}
