using System;

namespace Nubud.Models
{
    public class ArticleModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string Uri { get; set; }

        public string Image { get; set; }

        public string Domain { get; set; }

        public string[] Tags { get; set; }

        public bool IsFavorited { get; set; }

        public bool IsArchived { get; set; }
    }

    public class ArticleContentModel
    {
        public string ID { get; set; }
        public string Uri { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public string[] Tags { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string ArticleID { get; set; }
    }
}
