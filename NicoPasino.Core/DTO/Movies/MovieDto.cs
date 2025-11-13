namespace NicoPasino.Core.DTO.Movies
{
    public class MovieDto
    {
        public int? idPublica { get; set; }
        public string? title { get; set; }
        public int year { get; set; }
        public string? director { get; set; }
        public int duration { get; set; }
        public string? poster { get; set; }
        public IEnumerable<int>? genreIds { get; set; }
        public IEnumerable<string>? genreNames { get; set; }
        public decimal rate { get; set; }
    }
}