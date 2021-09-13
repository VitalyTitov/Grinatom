using Grinatom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Grinatom.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext db;
        public HomeController(ApplicationContext context)
        {
            db = context;
        }

        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> Index()
        {
            string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            
            if (role == "admin")
            {
                return View(await db.Users.ToListAsync());
            }
            else if (role == "user")
            {
                // id пользователя для отчета                
                string userId = User.FindFirst(ClaimsIdentity.DefaultNameClaimType).Value;
                User user = await db.Users.FirstOrDefaultAsync(p => p.Email == userId);
                if (user != null)
                    return View("UserWork", user);
            } 

            return NotFound();
        }

        //Вывод отчетов
        [Authorize(Roles = "admin")]
        public  ActionResult Report(int? id)
        {
            if (id != null)
            {
                bool last = false;
                TimeSpan timeSpan = new TimeSpan();
                var reports =  db.Reports.Where(p => p.UserId == id);
                IQueryable<DailyReport> daily = Enumerable.Empty<DailyReport>().AsQueryable();                
                //список дат
                var dataReport = reports
                          .Select(i => i.Date)
                          .Distinct()
                          .ToList();                
                
                //создание списка репортов для выбранного пользователя
                for (int i = 0; i < dataReport.Count(); i++)
                {
                    int count = reports.Count();
                    foreach (var item in reports)
                    {
                        count--;
                        if (item.Date == dataReport.ElementAt(i))
                        {
                            timeSpan += item.EndTime - item.StartTime;
                            last = true;
                        }
                        else if (last == true) 
                        {
                            var output = timeSpan.ToString(@"hh\:mm\:ss");
                            DailyReport dailyR = new DailyReport { Name = item.Name, Date = dataReport.ElementAt(i), Hours = output, Timesheet = item.Timesheet };
                            daily = daily.Concat(new[] { dailyR });
                            timeSpan = TimeSpan.Zero;
                            last = false;
                        }

                        if (count ==0 && i == dataReport.Count() - 1)
                        {
                            var output = timeSpan.ToString(@"hh\:mm\:ss");
                            DailyReport dailyR = new DailyReport { Name = item.Name, Date = dataReport.ElementAt(i), Hours = output, Timesheet = item.Timesheet };
                            daily = daily.Concat(new[] { dailyR });
                            timeSpan = TimeSpan.Zero;
                            last = false;
                        }
                    }                    
                }

                if (daily != null)
                    return View(daily);
            }
            return NotFound();                      
        }

        //Начало работы
        [HttpPost("/getReport")]
        public IActionResult GetReport([FromForm] int userId)
        {
            User user =  db.Users.FirstOrDefault(p => p.Id == userId);
            Report report = new Report {UserId = userId ,Name = user.FirstName + " " + user.LastName + " " + user.Patronymic,
                Timesheet = user.Timesheet, StartTime = DateTime.Now, EndTime = DateTime.Now, Date= DateTime.Now.ToShortDateString()
            };
            
            db.Reports.Add(report);
            db.SaveChanges();
            return  Ok(report.Id);
        }

        //Конец работы
        [HttpPost("/getReportEnd")]
        public IActionResult GetReportEnd([FromForm] int reportId)
        {
            Report report = db.Reports.FirstOrDefault(p => p.Id == reportId);
            report.EndTime = DateTime.Now;

            db.Reports.Update(report);
            db.SaveChangesAsync();

            return Ok("Работа закончена");
        }

        //Создание пользователей
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Редактирование пользователей
        public async Task<IActionResult> Edit(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            db.Users.Update(user);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
        //Удаление пользователей
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDelete(int? id)
        {
            if (id != null)
            {
                User user = await db.Users.FirstOrDefaultAsync(p => p.Id == id);
                if (user != null)
                    return View(user);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id != null)
            {
                User user = new User { Id = id.Value };
                db.Entry(user).State = EntityState.Deleted;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        [Authorize(Roles = "admin")]
        public IActionResult About()
        {
            return Content("Вход только для администратора");
        }

    }
}
