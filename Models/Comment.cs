﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentsTest.Models
{
    public class Comment
    {
        public Comment()
        {
            this.Comments = new HashSet<Comment>();
        }
        public int ID { get; set; }
        public string Text { get; set; }
        public int UserID { get; set; }
        public int ArticleID { get; set; }
        public Nullable<int> ParentID { get; set; }

        public virtual User User { get; set; }
        public virtual Comment ParentComment { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}