using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ekartes.DAL;
using Ekartes.Models;
using System.Data.Entity.Infrastructure;
using ekartes.CustomLibraries;
using System.Security.Claims;
using CaptchaMvc.HtmlHelpers;

namespace ekartes.Controllers
{
    public class DikaiouxosController : Controller
    {
        private ekartesDbContext db = new ekartesDbContext();

        // GET: Dikaiouxos/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        [AllowAnonymous]
        // POST: Dikaiouxos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,AM,Onoma,Epitheto,AT,Monada,Vathmos,O_S,KatastasiD,Rolos,Email,Password,ConfirmPassword")] Dikaiouxos dikaiouxos, HttpPostedFileBase upload, HttpPostedFileBase upload2)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var queryUser = db.Dikaiouxos.FirstOrDefault(u => u.Email == dikaiouxos.Email);
                    if (queryUser == null)
                    {
                        var encryptedPassword = CustomEncrypt.Encrypt(dikaiouxos.Password);
                        dikaiouxos.Password = encryptedPassword;
                        var encryptedConfirmPassword = CustomEncrypt.Encrypt(dikaiouxos.ConfirmPassword);
                        dikaiouxos.ConfirmPassword = encryptedConfirmPassword;
                    }
                    else
                    {
                        return RedirectToAction("Create");
                    }

                    if (upload != null && upload.ContentLength > 0 && upload2 != null && upload2.ContentLength > 0)
                    {
                        var pistopoiitiko = new FileDikaiouxos
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Pistopoiitiko,
                            ContentType = upload.ContentType,
                            Dikaiouxos = dikaiouxos
                        };

                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            pistopoiitiko.Content = reader.ReadBytes(upload.ContentLength);
                        }


                        var fATayt_d = new FileDikaiouxos
                        {
                            FileName = System.IO.Path.GetFileName(upload2.FileName),
                            FileType = FileType.FATayt_d,
                            ContentType = upload2.ContentType,
                            Dikaiouxos = dikaiouxos
                        };

                        using (var reader = new System.IO.BinaryReader(upload2.InputStream))
                        {
                            fATayt_d.Content = reader.ReadBytes(upload2.ContentLength);
                        }

                        db.FilesDikaiouxos.Add(pistopoiitiko);
                        db.FilesDikaiouxos.Add(fATayt_d);
                    }
                    else
                    {
                        ViewBag.Error = "Απαιτείται η υποβολή των παρακάτω δικαιολογητικών";
                        return View("Create");
                    }

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.create_d,
                        Katastasi = Katastasi.ekremmei,
                        Dikaiouxos = dikaiouxos
                    };
                    dikaiouxos.Rolos = Rolos.User;
                    db.Aitimata.Add(aitima);
                    db.Dikaiouxos.Add(dikaiouxos);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Τα στοιχεία που καταχωρήσατε δεν είναι σωστά.");
                }
            }


            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(dikaiouxos);
        }

        // GET: /Dikaiouxos/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //POST : /Dikaiouxos/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Dikaiouxos model, string empty)
        {


            if (model.Email == "admin@admin.com" && model.Password == "123456")
            {
                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name,"Developer"),
                        new Claim(ClaimTypes.Email,"admin@email.com")
                    },
                "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;

                authManager.SignIn(identity);

                return RedirectToAction("Home", "Admin");
            }

            var emailCheck = db.Dikaiouxos.FirstOrDefault(u => u.Email == model.Email);

            if (emailCheck == null)
            {
                ModelState.AddModelError("", "Μη αποδεκτό email ή κωδικός πρόσβασης");
                return View("Login");
            }

            var getPassword = db.Dikaiouxos.Where(u => u.Email == model.Email).Select(u => u.Password);
            var materializePassword = getPassword.ToList();
            var password = materializePassword[0];
            var decryptedPassword = CustomDecrypt.Decrypt(password);

            var getid = db.Dikaiouxos.Where(u => u.Email == model.Email).Select(u => u.ID);
            var materializeGetid = getid.ToList();
            var ident = materializeGetid[0];

            model.ID = ident;

            var getRolos = db.Dikaiouxos.Where(u => u.Email == model.Email).Select(u => u.Rolos);
            var materializeGetRolos = getRolos.ToList();
            var rolos = materializeGetRolos[0];

            model.Rolos = rolos;

            if (model.Rolos == Rolos.Admin)
            {
                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name,"Admin"),
                        new Claim(ClaimTypes.Email, model.Email)
                    },
                "ApplicationCookie");

                if (this.IsCaptchaValid("Captcha is valid"))
                {
                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;

                    authManager.SignIn(identity);

                    return RedirectToAction("Home", "Admin", new { id = model.ID });
                }

                else
                {
                    ModelState.AddModelError("", "Μη αποδεκτός Κωδικός Ασφαλείας.");
                    return View("Login");
                } 
            }


            if (model.Email != null && model.Password == decryptedPassword)
            {
                var getEmail = db.Dikaiouxos.Where(u => u.Email == model.Email).Select(u => u.Email);
                var materializeEmail = getEmail.ToList();
                var email = materializeEmail[0];


                var identity = new ClaimsIdentity(new[] {
                        new Claim(ClaimTypes.Name,email)
                    },
               "ApplicationCookie");

                if (this.IsCaptchaValid("Captcha is valid"))
                { 
                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;

                    authManager.SignIn(identity);

                    return RedirectToAction("ArxikoMenou", "Dikaiouxos", new { id = model.ID });
                }                   

                else
                {
                    ModelState.AddModelError("", "Μη αποδεκτός Κωδικός Ασφαλείας.");
                    return View("Login");
                }                
            }

            ModelState.AddModelError("", "Μη αποδεκτό email η Συνθηματικό.");
            return View(model);
        }

        // GET: Dikaiouxos/Logout
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Dikaiouxos");
        }

        // GET: Dikaiouxos/Manage
        public ActionResult Manage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);

            var pass = dikaiouxos.Password;
            var decrPass = CustomDecrypt.Decrypt(pass);

            dikaiouxos.Password = decrPass;

            if (dikaiouxos == null)
            {
                return HttpNotFound();
            }
            return View(dikaiouxos);
        }

        // POST: Dikaiouxos/Manage
        [HttpPost, ActionName("Manage")]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(int id)
        {

            var dikaiouxosToUpdate = db.Dikaiouxos.Find(id);
            if (TryUpdateModel(dikaiouxosToUpdate, "", new string[] { "Onoma", "Epitheto", "AM", "AT", "Monada", "Vathmos", "O_S", "KatastasiD", "Rolos", "Email", "Password","ConfirmPassword" }))
            {
                try
                {
                    var queryUser = db.Dikaiouxos.FirstOrDefault(u => u.Email == dikaiouxosToUpdate.Email);

                        if (queryUser != null)
                        {
                            var encryptPass = CustomEncrypt.Encrypt(dikaiouxosToUpdate.Password);
                            dikaiouxosToUpdate.Password = encryptPass;

                            var encryptConfirmPass = CustomEncrypt.Encrypt(dikaiouxosToUpdate.ConfirmPassword);
                            dikaiouxosToUpdate.ConfirmPassword = encryptConfirmPass;
                        }

                        else 
                        {
                            ViewBag.Error1 = "Αυτό το mail που καταχωρήσατε χρησιμοποιείται.";
                            return RedirectToAction("Manage", new { id = id });
                        }

                    db.Entry(dikaiouxosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                   
                    return RedirectToAction("Logout");
                }

                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(dikaiouxosToUpdate);
        }

        // GET: Dikaiouxos/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);
            if (dikaiouxos == null)
            {
                return HttpNotFound();
            }
            return View(dikaiouxos);
        }

        // POST: Dikaiouxos/Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, HttpPostedFileBase upload, HttpPostedFileBase upload2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dikaiouxosToUpdate = db.Dikaiouxos.Find(id);
            dikaiouxosToUpdate.ConfirmPassword = dikaiouxosToUpdate.Password;
            if (TryUpdateModel(dikaiouxosToUpdate, "", new string[] {"Onoma", "Epitheto", "AM", "AT", "Monada", "Vathmos", "O_S", "KatastasiD" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0 && upload2 != null && upload2.ContentLength > 0)
                    {

                        if (dikaiouxosToUpdate.FilesDikaiouxos.Any(f => f.FileType == FileType.Pistopoiitiko))
                        {
                            db.FilesDikaiouxos.Remove(dikaiouxosToUpdate.FilesDikaiouxos.First(f => f.FileType == FileType.Pistopoiitiko));
                        }

                        if (dikaiouxosToUpdate.FilesDikaiouxos.Any(f => f.FileType == FileType.FATayt_d))
                        {
                            db.FilesDikaiouxos.Remove(dikaiouxosToUpdate.FilesDikaiouxos.First(f => f.FileType == FileType.FATayt_d));
                        }

                        var pistopoiitiko = new FileDikaiouxos
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.Pistopoiitiko,
                            ContentType = upload.ContentType
                        };

                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            pistopoiitiko.Content = reader.ReadBytes(upload.ContentLength);
                        }

                        var fATayt_d = new FileDikaiouxos
                        {
                            FileName = System.IO.Path.GetFileName(upload2.FileName),
                            FileType = FileType.FATayt_d,
                            ContentType = upload2.ContentType
                        };

                        using (var reader = new System.IO.BinaryReader(upload2.InputStream))
                        {
                            fATayt_d.Content = reader.ReadBytes(upload2.ContentLength);
                        }

                        dikaiouxosToUpdate.FilesDikaiouxos.Add(pistopoiitiko);
                        dikaiouxosToUpdate.FilesDikaiouxos.Add(fATayt_d);

                    }

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.edit_d,
                        Katastasi = Katastasi.ekremmei,
                        Dikaiouxos = dikaiouxosToUpdate
                    };
                    db.Aitimata.Add(aitima);
                    db.Entry(dikaiouxosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("ArxikoMenou", new { id = dikaiouxosToUpdate.ID});
                }

                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
                return View(dikaiouxosToUpdate);
        }

        // GET: Dikaiouxos/FilePreview
        public ActionResult FilePreview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fileToRetrieve = db.FilesDikaiouxos.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }

        // GET: Dikaiouxos/ArxikoMenou
        public ActionResult ArxikoMenou(int id)
        {
            Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);
            var aitima = from a in db.Aitimata
                         //where (a.Eidos == Eidos.create_d) && (a.Dikaiouxos == dikaiouxos) && (a.Katastasi == Katastasi.ekremmei)
                         select a;

            int flag = 0;
            foreach(var a in aitima)
            {
                if ((a.Dikaiouxos == dikaiouxos && a.Katastasi == Katastasi.apodekto && a.Eidos == Eidos.create_d) || (a.Katastasi == Katastasi.anyparkto && a.Dikaiouxos == dikaiouxos && a.Eidos == Eidos.create_d))
                {
                    flag = 1;
                }
                
            }
            ViewBag.Flag = flag;
            return View(dikaiouxos);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
