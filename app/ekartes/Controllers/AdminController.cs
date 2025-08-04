using Ekartes.DAL;
using Ekartes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity.Infrastructure;
using ekartes.CustomLibraries;
using System.Data.Entity;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ekartes.Controllers
{
    public class AdminController : Controller
    {
        private ekartesDbContext db = new ekartesDbContext();
        
        //BarcodeImage
        public ActionResult BarcodeImage(String barcodeText)
        {
            QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
            QrCode qrCode = new QrCode();
            qrEncoder.TryEncode(barcodeText, out qrCode);
            GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(4, QuietZoneModules.Four), Brushes.Black, Brushes.White);

            Stream memoryStream = new MemoryStream();
            renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, memoryStream);

            memoryStream.Position = 0;

            var resultStream = new FileStreamResult(memoryStream, "image/png");
            resultStream.FileDownloadName = String.Format("{0}.png", barcodeText);

            return resultStream;
        }

        //MakeUser
        public ActionResult MakeUser(int id)
        {
            Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);
            dikaiouxos.ConfirmPassword = dikaiouxos.Password;
            dikaiouxos.Rolos = Rolos.User;
            db.SaveChanges();
            return RedirectToAction("Dikaiouxoi");
        }

        //MakeAdmin
        public ActionResult MakeAdmin(int id)
        {
           Dikaiouxos dikaiouxos =  db.Dikaiouxos.Find(id);
           dikaiouxos.ConfirmPassword = dikaiouxos.Password;
           dikaiouxos.Rolos = Rolos.Admin;
           db.SaveChanges();
          return  RedirectToAction("Dikaiouxoi");
        }

        //Home
        public ActionResult Home()
        {
            var aitimata = from a in db.Aitimata
                           where a.Katastasi == Katastasi.ekremmei
                           select a;
            return View(aitimata.ToList());
        }

        //GeneratePDF
        public ActionResult GeneratePDF(int id)
        {
            Melos melos = db.Melos.Find(id);
            return new Rotativa.ActionAsPdf("Ektyposi", new { id = melos.ID });
        }

        //[AllowAnonymous]
        public ActionResult Ektyposi(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Melos melos = db.Melos.Find(id);

            string[] HmniaEkdosis = melos.HmniaEkdosis.ToString().Split(new char[0]);
            ViewBag.HmniaEkdosis = HmniaEkdosis[0];

            string[] HmniaLiksis = melos.HmniaLiksis.ToString().Split(new char[0]);
            ViewBag.HmniaLiksis = HmniaLiksis[0];

            if (melos == null)
            {
                return HttpNotFound();
            }

            return View(melos);
        }

        //Aitimata
        public ActionResult Aitimata()
        {
            var aitimata = from a in db.Aitimata
                           where a.Katastasi == Katastasi.ekremmei
                           select a;

            return View(aitimata.ToList());
        }

        //AitimataCreateD
        public ActionResult AitimataCreateD()
        {
            var aitimata = from a in db.Aitimata
                           where (a.Eidos == Eidos.create_d) && (a.Katastasi == Katastasi.ekremmei)
                           select a;

            return View(aitimata.ToList());
        }

        //AitimataProsApodoxiDCreate
        public ActionResult AitimataProsApodoxiDCreate(int id)
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

        //AitimataEditD
        public ActionResult AitimataEditD()
        {
            var aitimata = from a in db.Aitimata
                           where (a.Eidos == Eidos.edit_d) && (a.Katastasi == Katastasi.ekremmei)
                           select a;

            return View(aitimata.ToList());
        }

        //AitimataProsApodoxiDEdit
        public ActionResult AitimataProsApodoxiDEdit(int id)
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

        //AitimataCreateM
        public ActionResult AitimataCreateM()
        {
            var aitimata = from a in db.Aitimata
                           where (a.Eidos == Eidos.create_m) && (a.Katastasi == Katastasi.ekremmei)
                           select a;

            return View(aitimata.ToList());
        }

        //AitimataProsApodoxiMCreate
        public ActionResult AitimataProsApodoxiMCreate(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Melos melos = db.Melos.Find(id);
            if (melos == null)
            {
                return HttpNotFound();
            }
            return View(melos);
        }

        //AitimataAnaneosiM
        public ActionResult AitimataAnaneosiM()
        {
            var aitimata = from a in db.Aitimata
                           where (a.Eidos == Eidos.ananeosi_m) && (a.Katastasi == Katastasi.ekremmei)
                           select a;

            return View(aitimata.ToList());
        }

        //AitimataProsApodoxiMAnaneosi
        public ActionResult AitimataProsApodoxiMAnaneosi(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Melos melos = db.Melos.Find(id);
            if (melos == null)
            {
                return HttpNotFound();
            }
            return View(melos);
        }

        //AitimataEpanekdosiM
        public ActionResult AitimataEpanekdosiM()
        {
            var aitimata = from a in db.Aitimata
                           where (a.Eidos == Eidos.epanekdosi_m) && (a.Katastasi == Katastasi.ekremmei)
                           select a;

            return View(aitimata.ToList());
        }

        //AitimataProsApodoxiMEpanekdosi
        public ActionResult AitimataProsApodoxiMEpanekdosi(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Melos melos = db.Melos.Find(id);
            if (melos == null)
            {
                return HttpNotFound();
            }
            return View(melos);
        }

        //Apodekto
        public ActionResult Apodekto(int id)
        {
            var aitima = db.Aitimata.Find(id);
            aitima.Katastasi = Katastasi.apodekto;
            db.SaveChanges();

            return View(aitima);
        }

        //MiApodekto
        public ActionResult MiApodekto(int id)
        {
            var aitima = db.Aitimata.Find(id);
            aitima.Katastasi = Katastasi.oxi_apodekto;
            db.SaveChanges();
            return View(aitima);
        }

        //DetailsDikaiouxou
        public ActionResult DetailsDikaiouxou(int id)
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

        //DetailsMelous
        public ActionResult DetailsMelous(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Melos melos = db.Melos.Find(id);
            if (melos == null)
            {
                return HttpNotFound();
            }
            return View(melos);
        }

        //Meli
        public ActionResult Meli(int? page, DateTime? fromDate, DateTime? toDate)
        {
            if (! fromDate.HasValue)
            {
                fromDate = DateTime.MinValue.Date;
            }
            if (!toDate.HasValue)
            {
                toDate = DateTime.Now.Date;
            }
            if (toDate < fromDate)
            {
                toDate = DateTime.Now.Date;
            }
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            var meli = db.Melos.Where(m => m.HmniaEkdosis >= fromDate && m.HmniaEkdosis <= toDate).ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(meli.ToPagedList(pageNumber, pageSize));
        }

        //MeliDikaiouxou
        public ActionResult MeliDikaiouxou(int? id)
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


            var meli = from m in db.Melos
                       select m;

            meli = meli.Where(m => m.Dikaiouxos.ID == id);

            return View(meli);
        }

        //Dikaiouxoi
        public ActionResult Dikaiouxoi(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.EpithetoSortParam = String.IsNullOrEmpty(sortOrder) ? "epitheto_desc" : "";
            ViewBag.AMSortParam = sortOrder == "AM" ? "AM_desc" : "AM";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var dikaiouxoi = from d in db.Dikaiouxos
                             select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                dikaiouxoi = dikaiouxoi.Where(d => d.Epitheto.Contains(searchString)
                    || d.Onoma.Contains(searchString) || d.AT.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "epitheto_desc":
                    dikaiouxoi = dikaiouxoi.OrderByDescending(s => s.Epitheto);
                    break;
                case "AM":
                    dikaiouxoi = dikaiouxoi.OrderBy(s => s.AM);
                    break;
                case "AM_desc":
                    dikaiouxoi = dikaiouxoi.OrderByDescending(s => s.AM);
                    break;
                default:
                    dikaiouxoi = dikaiouxoi.OrderBy(s => s.AM);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(dikaiouxoi.ToPagedList(pageNumber, pageSize));
        }

        // GET: Melos/Create
        public ActionResult CreateMelos(int? id)
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
            ViewBag.DikaiouxosID = id;
            return View();
        }

        // POST: Melos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMelos([Bind(Include = "ID,AT,Onoma,Epitheto,Sygeneia,HmniaEkdosis,HmniaLiksis,KwdikosKartas,Aitiologia")] Melos melos, int? id, HttpPostedFileBase upload, HttpPostedFileBase upload2)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (upload != null && upload.ContentLength > 0 && upload2 != null && upload2.ContentLength > 0)
                    {
                        var fATayt_m = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.FATayt_m,
                            ContentType = upload.ContentType,
                            Melos = melos
                        };

                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            fATayt_m.Content = reader.ReadBytes(upload.ContentLength);
                        }

                        var Photo = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload2.FileName),
                            FileType = FileType.Photo_m,
                            ContentType = upload2.ContentType,
                            Melos = melos
                        };

                        using (var reader = new System.IO.BinaryReader(upload2.InputStream))
                        {
                            Photo.Content = reader.ReadBytes(upload2.ContentLength);
                        }
                        db.FilesMelos.Add(fATayt_m);
                        db.FilesMelos.Add(Photo);
                    }
                    else
                    {
                        ViewBag.Error = "Απαιτείται η υποβολή των παρακάτω δικαιολογητικών";
                        return View("Create");
                    }

                    Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);
                    melos.Dikaiouxos = dikaiouxos;

                    int katastasi = (int)dikaiouxos.KatastasiD;
                    int am = dikaiouxos.AM;
                    string at = melos.AT;
                    string kodikos = katastasi.ToString() + "/" + am.ToString() + "/" + at;
                    melos.KwdikosKartas = kodikos;


                    melos.HmniaEkdosis = DateTime.Now.Date;
                    melos.HmniaLiksis = melos.HmniaEkdosis.AddYears(5);

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.create_m,
                        Katastasi = Katastasi.apodekto,
                        Dikaiouxos = dikaiouxos,
                        Melos = melos

                    };

                    db.Aitimata.Add(aitima);
                    db.Melos.Add(melos);
                    db.SaveChanges();
                    return RedirectToAction("Dikaiouxoi");
                }
            }

            catch (RetryLimitExceededException /* dex */)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(melos);
        }

        // GET : Melos/Ananeosi
        public ActionResult Ananeosi(int id)
        {
            Melos melos = db.Melos.Find(id);
            if (melos == null)
            {
                return HttpNotFound();
            }

            return View(melos);
        }

        // POST : Melos/Ananeosi
        [HttpPost, ActionName("Ananeosi")]
        [ValidateAntiForgeryToken]
        public ActionResult Ananeosi(int id, HttpPostedFileBase upload, HttpPostedFileBase upload2)
        {

            var melosToUpdate = db.Melos.Find(id);

            if (TryUpdateModel(melosToUpdate, "", new string[] { "ID", "AT", "Onoma", "Epitheto", "Sygeneia", "HmniaEkdosis", "HmniaLiksis", "KwdikosKartas", "Aitiologia" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0 && upload2 != null && upload2.ContentLength > 0)
                    {

                        if (melosToUpdate.FilesMelos.Any(f => f.FileType == FileType.FATayt_m))
                        {
                            db.FilesMelos.Remove(melosToUpdate.FilesMelos.First(f => f.FileType == FileType.FATayt_m));
                        }

                        if (melosToUpdate.FilesMelos.Any(f => f.FileType == FileType.Photo_m))
                        {
                            db.FilesMelos.Remove(melosToUpdate.FilesMelos.First(f => f.FileType == FileType.Photo_m));
                        }

                        var fATayt_m = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.FATayt_m,
                            ContentType = upload.ContentType,
                            Melos = melosToUpdate
                        };

                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            fATayt_m.Content = reader.ReadBytes(upload.ContentLength);
                        }

                        var Photo = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload2.FileName),
                            FileType = FileType.Photo_m,
                            ContentType = upload2.ContentType,
                            Melos = melosToUpdate
                        };

                        using (var reader = new System.IO.BinaryReader(upload2.InputStream))
                        {
                            Photo.Content = reader.ReadBytes(upload2.ContentLength);
                        }

                        melosToUpdate.FilesMelos.Add(fATayt_m);
                        melosToUpdate.FilesMelos.Add(Photo);

                    }
                    melosToUpdate.HmniaEkdosis = DateTime.Now.Date;
                    melosToUpdate.HmniaLiksis = melosToUpdate.HmniaEkdosis.AddYears(5);

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.ananeosi_m,
                        Katastasi = Katastasi.apodekto,
                        Dikaiouxos = melosToUpdate.Dikaiouxos,
                        Melos = melosToUpdate
                    };

                    db.Aitimata.Add(aitima);
                    db.Entry(melosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("MeliDikaiouxou", "Admin", new { id = melosToUpdate.Dikaiouxos.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(melosToUpdate);
        }

        // GET : Melos/Epanekdosi
        public ActionResult Epanekdosi(int id)
        {
            Melos melos = db.Melos.Find(id);
            if (melos == null)
            {
                return HttpNotFound();
            }
            return View(melos);
        }

        // POST : Melos/Epanekdosi
        [HttpPost, ActionName("Epanekdosi")]
        [ValidateAntiForgeryToken]
        public ActionResult Epanekdosi(int id, HttpPostedFileBase upload, HttpPostedFileBase upload2, HttpPostedFileBase upload3)
        {
            var melosToUpdate = db.Melos.Find(id);

            if (TryUpdateModel(melosToUpdate, "", new string[] { "ID", "AT", "Onoma", "Epitheto", "Sygeneia", "HmniaEkdosis", "HmniaLiksis", "KwdikosKartas", "Aitiologia" }))
            {
                try
                {
                    if (upload != null && upload.ContentLength > 0 && upload2 != null && upload2.ContentLength > 0)
                    {

                        if (melosToUpdate.FilesMelos.Any(f => f.FileType == FileType.FATayt_m))
                        {
                            db.FilesMelos.Remove(melosToUpdate.FilesMelos.First(f => f.FileType == FileType.FATayt_m));
                        }

                        if (melosToUpdate.FilesMelos.Any(f => f.FileType == FileType.Photo_m))
                        {
                            db.FilesMelos.Remove(melosToUpdate.FilesMelos.First(f => f.FileType == FileType.Photo_m));
                        }


                        var fATayt_m = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload.FileName),
                            FileType = FileType.FATayt_m,
                            ContentType = upload.ContentType,
                            Melos = melosToUpdate
                        };

                        using (var reader = new System.IO.BinaryReader(upload.InputStream))
                        {
                            fATayt_m.Content = reader.ReadBytes(upload.ContentLength);
                        }

                        var Photo = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload2.FileName),
                            FileType = FileType.Photo_m,
                            ContentType = upload2.ContentType,
                            Melos = melosToUpdate
                        };

                        using (var reader = new System.IO.BinaryReader(upload2.InputStream))
                        {
                            Photo.Content = reader.ReadBytes(upload2.ContentLength);
                        }

                        melosToUpdate.FilesMelos.Add(fATayt_m);
                        melosToUpdate.FilesMelos.Add(Photo);

                    }

                    if (upload3 != null && upload3.ContentLength > 0)
                    {
                        if (melosToUpdate.FilesMelos.Any(f => f.FileType == FileType.Dikaiologitiko_Epanekdosis))
                        {
                            db.FilesMelos.Remove(melosToUpdate.FilesMelos.First(f => f.FileType == FileType.Dikaiologitiko_Epanekdosis));
                        }
                        var dikaiologitiko = new FileMelos
                        {
                            FileName = System.IO.Path.GetFileName(upload3.FileName),
                            FileType = FileType.Dikaiologitiko_Epanekdosis,
                            ContentType = upload3.ContentType,
                            Melos = melosToUpdate
                        };

                        using (var reader = new System.IO.BinaryReader(upload3.InputStream))
                        {
                            dikaiologitiko.Content = reader.ReadBytes(upload3.ContentLength);
                        }

                        melosToUpdate.FilesMelos.Add(dikaiologitiko);
                    }
                    else
                    {
                        ViewBag.Error = "Απαιτείται η υποβολή του παρακάτω δικαιολογητικού";
                        return View("Epanekdosi", new { id = id });
                    }

                    melosToUpdate.HmniaEkdosis = DateTime.Now.Date;
                    melosToUpdate.HmniaLiksis = melosToUpdate.HmniaEkdosis.AddYears(5);

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.epanekdosi_m,
                        Katastasi = Katastasi.apodekto,
                        Dikaiouxos = melosToUpdate.Dikaiouxos,
                        Melos = melosToUpdate
                    };

                    db.Aitimata.Add(aitima);
                    db.Entry(melosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("MeliDikaiouxou", "Admin", new { id = melosToUpdate.Dikaiouxos.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(melosToUpdate);
        }

        // GET: Admin/CreateDikaiouxos
        [AllowAnonymous]
        public ActionResult CreateDikaiouxos()
        {
            return View();
        }

        // POST: Admin/CreateDikaiouxos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDikaiouxos([Bind(Include = "ID,AM,Onoma,Epitheto,AT,Monada,Vathmos,O_S,KatastasiD,Rolos,Email,Password,ConfirmPassword")] Dikaiouxos dikaiouxos, HttpPostedFileBase upload, HttpPostedFileBase upload2)
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
                    }
                    else
                    {
                        return RedirectToAction("CreateDikaiouxos");
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
                        return View("CreateDikaiouxos");
                    }

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.create_d,
                        Katastasi = Katastasi.anyparkto,
                        Dikaiouxos = dikaiouxos
                    };

                    db.Aitimata.Add(aitima);
                    dikaiouxos.Rolos = Rolos.User;
                    dikaiouxos.ConfirmPassword = dikaiouxos.Password;
                    db.Dikaiouxos.Add(dikaiouxos);
                    db.SaveChanges();
                    return RedirectToAction("Home");
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

        // GET: Dikaiouxos/EditD
        public ActionResult EditD(int? id)
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

        // POST: Dikaiouxos/EditD
        [HttpPost, ActionName("EditD")]
        [ValidateAntiForgeryToken]
        public ActionResult EditD(int? id, HttpPostedFileBase upload, HttpPostedFileBase upload2)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var dikaiouxosToUpdate = db.Dikaiouxos.Find(id);
            dikaiouxosToUpdate.ConfirmPassword = dikaiouxosToUpdate.Password;
            if (TryUpdateModel(dikaiouxosToUpdate, "", new string[] { "Onoma", "Epitheto", "AM", "AT", "Monada", "Vathmos", "O_S", "KatastasiD", "Rolos", "Email"}))
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

                    db.Entry(dikaiouxosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Dikaiouxoi");
                }

                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(dikaiouxosToUpdate);
        }
    }
}