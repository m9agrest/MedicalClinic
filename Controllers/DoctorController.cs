using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalClinic.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MedicalClinic.Controllers
{
    public class DoctorController : Controller
    {
        private readonly MedicalClinicContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DoctorController(
            MedicalClinicContext context,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.webHostEnvironment = webHostEnvironment;
        }

        private Human? GetCurrentUser()
        {
            var userId = httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = context.Humans
                    .FirstOrDefault(i => i.Id == userId);

                if (user != null)
                {
                    return user;
                }

                httpContextAccessor.HttpContext.Session.Remove("UserId");
            }
            return null;
        }

        private bool IsAdmin() => GetCurrentUser()?.Type == 2; // 2 - администратор

        // 1. Список врачей
        [HttpGet]
        public IActionResult List()
        {
            var doctors = context.Humans
                .Include(h => h.HumanDoctorTypes).ThenInclude(hdt => hdt.DoctorType) // Включаем связанные типы врачей
                .Where(h => h.Type > 0)
                .ToList();

            return View(doctors);
        }

        // 2. Список типов врачей
        [HttpGet]
        public IActionResult TypeList()
        {
            var types = context.DoctorTypes.ToList();
            ViewBag.Admin = IsAdmin();
            return View(types);
        }

        // 3. Добавление типа врача (GET)
        [HttpGet]
        public IActionResult TypeAdd()
        {
            if (!IsAdmin())
            {
                return Forbid(); // Если не администратор, возвращаем 403
            }
            return View(new DoctorType());
        }

        // 4. Добавление типа врача (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TypeAdd(DoctorType model)
        {
            if (!IsAdmin())
            {
                return Forbid(); // Если не администратор, возвращаем 403
            }

            if (ModelState.IsValid)
            {
                context.DoctorTypes.Add(model);
                context.SaveChanges();
                return RedirectToAction("TypeList");
            }
            return View(model);
        }

        // 5. Обновление типа врача (GET)
        [HttpGet]
        public IActionResult TypeUpdate(int id)
        {
            if (!IsAdmin())
            {
                return Forbid(); // Если не администратор, возвращаем 403
            }

            var type = context.DoctorTypes.FirstOrDefault(t => t.Id == id);
            if (type == null)
            {
                return NotFound();
            }
            return View(type);
        }

        // 6. Обновление типа врача (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TypeUpdate(DoctorType model)
        {
            if (!IsAdmin())
            {
                return Forbid(); // Если не администратор, возвращаем 403
            }

            if (ModelState.IsValid)
            {
                context.DoctorTypes.Update(model);
                context.SaveChanges();
                return RedirectToAction("TypeList");
            }
            return View(model);
        }
    }
}
