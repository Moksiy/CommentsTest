using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace CommentsTest.Models
{
    public class CommentsRepository
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public List<Article> GetArticles()
        {
            List<Article> articles = new List<Article>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                articles = db.Query<Article>("SELECT * FROM Article").ToList();
            }
            return articles;
        }

        public List<Comment> GetComments(int articleId)
        {
            List<Comment> comments = new List<Comment>();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                comments = db.Query<Comment>("SELECT * FROM Comment WHERE Article = @articleId", new { articleId }).ToList();
            }
            return comments;
        }

        public Comment Add(Comment comment)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sqlQuery = "INSERT INTO Comments (Text, User, Article, ParentId) VALUES(@Text, @User, @Article, @ParentId); SELECT CAST(SCOPE_IDENTITY() as int)";
                int? commentId = db.Query<int>(sqlQuery, comment).FirstOrDefault();
                comment.ID = (int)commentId;
            }
            return comment;
        }
    }
}