using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestAplication.Data;
using TestAplication.Models;

namespace TestAplication.Controllers
{
    //[Authorize]
    public class SchedulesController : Controller
    {
        private readonly ScheduleContext _context;

        public SchedulesController(ScheduleContext context)
        {
            _context = context;
        }

        // GET: Schedules
        public async Task<IActionResult> Index(string sortOrder, string searchString, string FilterCategory)
        {
            //FormsAuthentication.SetAuthCookie(sortOrder, false);
            var schedules = from s in _context.Schedule
                            select s;

            var unDoneSchedules = from s in _context.Schedule
                            where s.IsDone == false
                            select s;
            foreach (var data in unDoneSchedules)
            {
                var dateDiff = data.DueDate - DateTimeOffset.Now;
                if (dateDiff.TotalMinutes <= 15)
                {
                    ViewData["AlertDue"] = "Some Task Will Be Due Soon or Over Due, Please Check if Done.";
                }
            }

            //ViewData["detailSortParm"] = String.IsNullOrEmpty(sortOrder) ? "detail_desc" : "";
            ViewData["priSortParm"] = String.IsNullOrEmpty(sortOrder) ? "priority_desc" : "";
            ViewData["dueDateSortParm"] = sortOrder == "Date" ? "dueDate_asc" : "Date";
            ViewData["CurrentFilter"] = searchString;
            ViewData["FilterCategory"] = FilterCategory;

            if (!string.IsNullOrEmpty(FilterCategory) && !String.IsNullOrEmpty(searchString))
            {
                switch (FilterCategory)
                {
                    case "Priority":
                        schedules = schedules.Where(s => s.Priority ==  Convert.ToInt32(searchString));
                        break;
                    case "Detail":
                        schedules = schedules.Where(s =>s.Detail.Contains(searchString.ToUpper()));
                        break;
                    default:
                        break;
                }
            }

            switch (sortOrder)
            {
                //case "detail_desc":
                //    schedules = schedules.OrderByDescending(s => s.Detail);
                //    break;
                case "priority_desc":
                    schedules = schedules.OrderByDescending(s => s.Priority);
                    break;
                case "dueDate_asc":
                    schedules = schedules.OrderBy(s => s.DueDate);
                    break;
                default:
                    schedules = schedules.OrderByDescending(s => s.DueDate);
                    break;
            }

            return View(await schedules.AsNoTracking().ToListAsync());
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Schedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Detail,Priority,DueDate,IsDone")] Schedule schedule)
        {
            using (var transaction = this._context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        schedule.Detail?.ToUpper();
                        schedule.IsDone = false;
                        if (schedule.Priority > 5)
                        {
                            ViewData["AlertPri"] = "Max Priority Number Is 5";
                        }
                        else if (schedule.DueDate < DateTimeOffset.Now)
                        {
                            ViewData["AlertDate"] = "Due Date Cannot Less than Now";
                        }
                        else
                        {
                            _context.Add(schedule);
                            await _context.SaveChangesAsync();
                            transaction.Commit();
                            return RedirectToAction(nameof(Index));
                        }

                    }
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
         
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Detail,Priority,DueDate,IsDone")] Schedule schedule)
        {
            if (id != schedule.Id)
            {
                return NotFound();
            }
            using (var transaction = this._context.Database.BeginTransaction())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            schedule.Detail?.ToUpper();

                            if (schedule.Priority > 5)
                            {
                                ViewData["AlertPri"] = "Max Priority Number Is 5";
                            }
                            else
                            {
                                _context.Update(schedule);
                                await _context.SaveChangesAsync();
                                transaction.Commit();
                            }

                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!ScheduleExists(schedule.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
              
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = this._context.Database.BeginTransaction())
            {
                try
                {
                    var schedule = await _context.Schedule.FindAsync(id);
                    _context.Schedule.Remove(schedule);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch(Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e.Message);
                }
            }
               
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(int id)
        {
            return _context.Schedule.Any(e => e.Id == id);
        }
    }
}
