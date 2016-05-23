using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ManyToMany.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }

        public virtual ICollection<AuthorToBook> AuthorsToBook { get; set; }

    }
}