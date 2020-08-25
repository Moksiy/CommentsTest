using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentsTest.Models
{
    public class CommentsModel
    {
        public Article Article { get; set; }

        public int? ReplyComment { get; set; }
    }
}