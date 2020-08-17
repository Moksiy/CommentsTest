using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CommentsTest.DAL;
using CommentsTest.Models;

namespace CommentsTest.Controllers
{
    public class ArticlesController : Controller
    {
        //private readonly BlogContext db = new BlogContext();

        CommentsRepository repo = new CommentsRepository();
        private const int ArticlesPerPage = 2;

        // GET: Articles
        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<Article> articles = repo.GetArticles()
                .Skip(pageNumber * ArticlesPerPage)
                .Take(ArticlesPerPage + 1);

            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = articles.Count() > ArticlesPerPage;
            ViewBag.PageNumber = pageNumber;

            return View(articles.Take(ArticlesPerPage));
        }

        // GET: Articles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Article article = repo.GetArticle(id);

            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        [ValidateInput(false)]
        public ActionResult Comment(int id, string text)
        {
            Article article = repo.GetArticle(id);
            Comment comment = new Comment();
            comment.ArticleID = article.ID;
            comment.Text = text;
            comment.ParentID = null;
            comment.UserID = 1;
            comment.User = new User { Name = "TEST" };
            repo.AddComment(comment);
            return RedirectToAction("Details", new { id = id });
        }

        // GET: Articles/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Text")] Article article)
        {
            if (ModelState.IsValid)
            {
                //db.Articles.Add(article);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(article);
        }

        // GET: Articles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = new Article();//db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Text")] Article article)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(article).State = EntityState.Modified;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = new Article();//db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed()
        {
            //Article article = db.Articles.Find(id);
            //db.Articles.Remove(article);
            //db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
