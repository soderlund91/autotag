namespace AutoTag
{
    public class ExternalItemDto
    {
        public string Name { get; set; }
        public string Imdb { get; set; }
        public string Tmdb { get; set; }
    }

    public class MdbListItem
    {
        public string title { get; set; }
        public string imdb_id { get; set; }
        public int? id { get; set; }
        public string media_type { get; set; }
    }

    public class TraktBaseObject
    {
        public int rank { get; set; }
        public string type { get; set; }
        public TraktMovie movie { get; set; }
        public TraktShow show { get; set; }
    }

    public class TraktMovie
    {
        public string title { get; set; }
        public int year { get; set; }
        public TraktIds ids { get; set; }
    }

    public class TraktShow
    {
        public string title { get; set; }
        public int year { get; set; }
        public TraktIds ids { get; set; }
    }

    public class TraktIds
    {
        public int trakt { get; set; }
        public string slug { get; set; }
        public int? tmdb { get; set; }
        public string imdb { get; set; }
    }
}