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
        public int UserID { get; set; }
        public int ArticleID { get; set; }
        public int? ParentID { get; set; }
    }
}