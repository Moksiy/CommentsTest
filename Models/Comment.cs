using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentsTest.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public Comment Parent { get; set; }
        public User User { get; set; }
        public Article Article { get; set; }
    }
}