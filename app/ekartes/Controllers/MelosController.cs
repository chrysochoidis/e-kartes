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
using System.Drawing;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System.IO;
using System.Drawing.Imaging;

namespace ekartes.Controllers
{
    public class MelosController : Controller
    {
        private ekartesDbContext db = new ekartesDbContext();

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

        public ActionResult FilePreview(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var fileToRetrieve = db.FilesMelos.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }

        public ActionResult MeliDikaiouxou(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);

            ViewBag.Dik_ID = dikaiouxos.ID;

            if (dikaiouxos == null)
            {
                return HttpNotFound();
            }


            var meli = from m in db.Melos
                       select m;

            meli = meli.Where(m => m.Dikaiouxos.ID == id);

            return View(meli);
        }

        public ActionResult MeliDikaiouxouEkt(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Dikaiouxos dikaiouxos = db.Dikaiouxos.Find(id);

            ViewBag.Dik_IDEkt = dikaiouxos.ID;

            if (dikaiouxos == null)
            {
                return HttpNotFound();
            }
            
            var aitimata = from a in db.Aitimata
                           where (a.Katastasi == Katastasi.apodekto) && (a.Melos.Dikaiouxos.ID == dikaiouxos.ID)
                           select a;

            return View(aitimata.ToList());
        }

        // GET: Melos/Details
        public ActionResult Details(int? id)
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

        // GET: Melos/Create
        public ActionResult Create(int? id)
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
        public ActionResult Create([Bind(Include = "ID,AT,Onoma,Epitheto,Sygeneia,HmniaEkdosis,HmniaLiksis,KwdikosKartas,Aitiologia")] Melos melos, int? id, HttpPostedFileBase upload, HttpPostedFileBase upload2)
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

                    //var dikaiouxos = db.Dikaiouxos.Find(melos.AMDikaiouxou);
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
                        Katastasi = Katastasi.ekremmei,
                        Dikaiouxos = dikaiouxos,
                        Melos = melos
                        
                    };

                    db.Aitimata.Add(aitima);
                    db.Melos.Add(melos);
                    db.SaveChanges();
                    return RedirectToAction("ArxikoMenou", "Dikaiouxos", new { id = dikaiouxos.ID });
                }
            }

            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            //ViewBag.AMDikaiouxou = new SelectList(db.Dikaiouxos, "AM", "AM", melos.AMDikaiouxou);
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


            //ViewBag.AMDikaiouxou = new SelectList(db.Dikaiouxos, "AM", "AM", melos.AMDikaiouxou);
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
                        Katastasi = Katastasi.ekremmei,
                        Dikaiouxos = melosToUpdate.Dikaiouxos,
                        Melos = melosToUpdate
                    };

                    db.Aitimata.Add(aitima);
                    db.Entry(melosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("MeliDikaiouxou", "Melos", new { id = melosToUpdate.Dikaiouxos.ID });
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

            if (TryUpdateModel(melosToUpdate, "", new string[] { "ID","AT","Onoma","Epitheto","Sygeneia","HmniaEkdosis","HmniaLiksis","KwdikosKartas","Aitiologia"}))
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
                        return View("Epanekdosi", new  { id = id});
                    }

                    melosToUpdate.HmniaEkdosis = DateTime.Now.Date;
                    melosToUpdate.HmniaLiksis = melosToUpdate.HmniaEkdosis.AddYears(5);

                    var aitima = new Aitima
                    {
                        Eidos = Eidos.epanekdosi_m,
                        Katastasi = Katastasi.ekremmei,
                        Dikaiouxos = melosToUpdate.Dikaiouxos,
                        Melos = melosToUpdate
                    };

                    db.Aitimata.Add(aitima);
                    db.Entry(melosToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("MeliDikaiouxou", "Melos", new { id = melosToUpdate.Dikaiouxos.ID });
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(melosToUpdate);
        }
       

        [AllowAnonymous]
        public ActionResult Ektyposi(int id)
        {
            Melos melos = db.Melos.Find(id);

            var aitimata = from a in db.Aitimata
                           where a.Melos != null
                           select a;

            foreach (var a in aitimata.ToList())
            {
                if (a.Katastasi == Katastasi.apodekto && a.Melos.ID == id)
                {
                    a.Katastasi = Katastasi.anyparkto;
                    db.SaveChanges();
                }
                
            }
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
