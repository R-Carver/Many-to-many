using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ManyToMany.Models;

namespace ManyToMany.Controllers
{
    public class AuthorsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Authors
        public ActionResult Index()
        {
            return View(db.Authors.ToList());
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AuthorID,Name,Surname")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Authors.Add(author);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(author);
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }

            var Results = from b in db.Books
                          select new
                          {
                              b.BookID,
                              b.Title,
                              Checked = ((from ab in db.AuthorsToBooks
                                          where (ab.AuthorID == id) & (ab.BookID == b.BookID)
                                          select ab).Count() > 0)
                          };

            var MyViewModel = new AuthorsViewModel();

            MyViewModel.AuthorID = id.Value;
            MyViewModel.Name = author.Name;
            MyViewModel.Surname = author.Surname;

            var MyCheckBoxList = new List<CheckBoxViewModel>();

            foreach(var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxViewModel { Id = item.BookID, Name = item.Title, Checked = item.Checked });
            }
            MyViewModel.Books = MyCheckBoxList;
            return View(MyViewModel);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuthorsViewModel author)
        {
            if (ModelState.IsValid)
            {
                var MyAuthor = db.Authors.Find(author.AuthorID);

                MyAuthor.Name = author.Name;
                MyAuthor.Surname = author.Surname;

                foreach(var item in db.AuthorsToBooks)
                {
                    if(item.AuthorID == author.AuthorID)
                    {
                        db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                }

                foreach(var item in author.Books)
                {
                    if (item.Checked)
                    {
                        db.AuthorsToBooks.Add(new AuthorToBook() { AuthorID = author.AuthorID, BookID = item.Id });
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Author author = db.Authors.Find(id);
            db.Authors.Remove(author);
            db.SaveChanges();
            return RedirectToAction("Index");
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
