using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bike.Web.Models;
using Bike.Web.AppDbContext;
using Microsoft.AspNetCore.Authorization;

namespace Bike.Web.Controllers
{
    [Authorize(Roles = "Admin, Executive")]
    public class MakeController : Controller
    {
        private readonly VroomDbContext _dbContext;
        public MakeController(VroomDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View(_dbContext.Makes.ToList());
        }

       
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Make make)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Add(make);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(make);
        }

        
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var make = _dbContext.Makes.Find(id);
            if(make == null)
            {
                return NotFound();
            }
            _dbContext.Makes.Remove(make);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var make = _dbContext.Makes.Find(id);
            if (make == null)
            {
                return NotFound();
            }
            return View(make);
        }

        [HttpPost]
        public IActionResult Edit(Make make)
        {
            if(ModelState.IsValid)
            {
                _dbContext.Update(make);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(make);
        }

    }
}
