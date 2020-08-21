using CommentsTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommentsTest.Controllers
{
    public class CommentsController : Controller
    {
        // GET: Comments
        public ActionResult Index(int id)
        {
            CommentsRepository repo = new CommentsRepository();

            CommentsModel model = new CommentsModel();

            model.Comments = repo.GetComments(id);



            return View();
        }
    }
}