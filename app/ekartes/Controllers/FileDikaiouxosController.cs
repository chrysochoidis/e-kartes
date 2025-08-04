using Ekartes.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ekartes.Controllers
{
    public class FileDikaiouxosController : Controller
    {
        private ekartesDbContext db = new ekartesDbContext();

        public ActionResult Index(int id)
        {
            var fileToRetrieve = db.FilesDikaiouxos.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
    }
}