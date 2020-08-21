using System;
using System.Collections.Generic;
using System.Data;
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

        readonly CommentsRepository repo = new CommentsRepository();
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

            CommentsModel model = new CommentsModel() { Article = repo.GetArticle((int)id)};

            if (model.Article == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult Comment(int id, string text, string name)
        {
            Article article = repo.GetArticle(id);
            User user = repo.CheckUser(name);

            Comment comment = new Comment
            {
                Article = article,
                Text = text,
                Parent = null,
                User = user
            };
            repo.AddComment(comment);
            return RedirectToAction("Details", new { id });
        }

        [ValidateInput(false)]
        public ActionResult ReplyComment(int id, string text, string name)
        {
            int articleID = repo.GetArticleID(id);
            Article article = repo.GetArticle(articleID);
            User user = repo.CheckUser(name);
            Comment parentComment = repo.GetComment(id);

            Comment comment = new Comment
            {
                Article = article,
                Text = text,
                Parent = parentComment,
                User = user
            };
            repo.AddComment(comment);

            id = articleID;

            return RedirectToAction("Details", new { id });
        }

        public ActionResult Reply(int? id)
        {
            CommentsModel model = new CommentsModel() { Article = repo.GetArticle(repo.GetArticleID((int)id)) };

            model.ReplyComment = id;

            return View("Details", model);
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
            Article article = new Article();
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
            Article article = new Article();
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
