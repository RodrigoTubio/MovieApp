namespace MoviesApp.Entities
{
    public class Movie
    {
        public int id { get; set; }

        public string? title { get; set; }

        public string? original_title { get; set; }

        public decimal vote_average { get; set; }

        public string? release_date { get; set; }

        public string? overview { get; set; }

    }
}
