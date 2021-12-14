using Bike.Web.AppDbContext;
using Bike.Web.Helpers;
using Bike.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Bike.Web.Models;
using cloudscribe.Pagination.Models;

namespace Bike.Web.Controllers
{
    [Authorize(Roles = Roles.Admin + "," + Roles.Executive)]
    public class BikeController : Controller
    {
        private readonly VroomDbContext _dbContext;
        private readonly IWebHostEnvironment _webHhostingEnvironment;

        [BindProperty]
        public MotorcycleViewModel BikeVM { get; set; }

        public BikeController(VroomDbContext dbContext, IWebHostEnvironment webHhostingEnvironment)
        {
            _dbContext = dbContext;
            _webHhostingEnvironment = webHhostingEnvironment;

            BikeVM = new MotorcycleViewModel()
            {
                Makes = _dbContext.Makes.ToList(),
                Models = _dbContext.Models.ToList(),
                Bike = new Models.Motorcycle()
            };
        }
        
        [AllowAnonymous]
        public IActionResult Index(string searchString, string sortOrder, int pageNumber = 1, int pageSize = 2)
        {
            ViewBag.PriceSortParam = String.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentFilter = searchString;
            int excludeRecords = (pageSize * pageNumber) - pageSize;
            var Bikes = from b in _dbContext.Bikes.Include(m => m.Make).Include(m => m.Model)
                        select b;
            var BikeCount = Bikes.Count();
            if (!String.IsNullOrEmpty(searchString))
            {
                Bikes = Bikes.Where(b => b.Make.Name.Contains(searchString));
                BikeCount = Bikes.Count();
            }

            //Sorting Logic
            switch(sortOrder)
            {
                case "price_desc":
                    Bikes = Bikes.OrderByDescending(b => b.Price);
                    break;
                default:
                    Bikes = Bikes.OrderBy(b => b.Price);
                    break;
            }
            Bikes = Bikes .Skip(excludeRecords)
                .Take(pageSize);
            var result = new PagedResult<Motorcycle>
            {
                Data = Bikes.AsNoTracking().ToList(),
                TotalItems= BikeCount,
                //TotalItems = _dbContext.Bikes.Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            return View(result);
        }

        public IActionResult Create()
        {
            return View(BikeVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryTokenAttribute]
        public IActionResult CreatePost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _dbContext.Makes.ToList();
                BikeVM.Models = _dbContext.Models.ToList();
                return View(BikeVM);
            }
            _dbContext.Bikes.Add(BikeVM.Bike);
            UploadImageIfAvailable();
            _dbContext.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            BikeVM.Bike = _dbContext.Bikes.SingleOrDefault(b => b.Id == id);

            //Filter the models associated to the selected make
            BikeVM.Models = _dbContext.Models.Where(m => m.MakeID == BikeVM.Bike.MakeID);

            if (BikeVM.Bike == null)
            {
                return NotFound();
            }
            return View(BikeVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost()
        {
            if (!ModelState.IsValid)
            {
                BikeVM.Makes = _dbContext.Makes.ToList();
                BikeVM.Models = _dbContext.Models.ToList();
                return View(BikeVM);
            }
            _dbContext.Update(BikeVM.Bike);
            UploadImageIfAvailable();
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Motorcy‎cle bike = _dbContext.Bikes.Find(id);
            if (bike == null)
            {
                return NotFound();
            }
            _dbContext.Bikes.Remove(bike);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult View(int id)
        {
            BikeVM.Bike = _dbContext.Bikes.SingleOrDefault(b => b.Id == id);

            if (BikeVM.Bike == null)
            {
                return NotFound();
            }
            return View(BikeVM);
        }

        private void UploadImageIfAvailable()
        {
            ///////////////////////////////
            //Save Bike Logic
            //////////////////////////////

            //Get Bike Id we have saved in database
            var BikeID = BikeVM.Bike.Id;

            //Get wwrootpath to save the file in server
            string wwrootpath = _webHhostingEnvironment.WebRootPath;

            //Get the upload files
            var files = HttpContext.Request.Form.Files;

            //Get the reference of DBSet for the bike we just have saved in database
            var SavedBike = _dbContext.Bikes.Find(BikeID);

            //Upload the files on server and save the image path of user have uploaded any file
            if (files.Count != 0)
            {
                var ImagePath = @"images\bike\";
                var Extension = Path.GetExtension(files[0].FileName);
                var RelativeImagePath = ImagePath + BikeID + Extension;
                var AbsImagePath = Path.Combine(wwrootpath, RelativeImagePath);

                //Upload the file on server
                using (var fileStream = new FileStream(AbsImagePath, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                //Set the image path on database
                SavedBike.ImagePath = RelativeImagePath;
            }
        }
    }
}
