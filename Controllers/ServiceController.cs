using MedicalClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace MedicalClinic.Controllers
{
    public class ServiceController : Controller
    {
        private readonly MedicalClinicContext context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ServiceController(
            MedicalClinicContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        private Human? GetCurrentUser()
        {
            var userId = httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = context.Humans.FirstOrDefault(i => i.Id == userId);
                if (user != null)
                {
                    return user;
                }
                httpContextAccessor.HttpContext.Session.Remove("UserId");
            }
            return null;
        }

        private bool IsAdmin() => GetCurrentUser()?.Type == 2; // 2 - администратор

        [HttpGet]
        public IActionResult Item(int id)
        {
            var service = context.Services
                .Include(s => s.ServiceDoctorTypes).ThenInclude(sdt => sdt.DoctorType) // Включаем связанные типы врачей
                .FirstOrDefault(s => s.Id == id);

            if (service == null) return NotFound();

            // Получаем врачей, которые могут оказывать услугу
            var doctors = context.Humans
                .Include(h => h.HumanDoctorTypes)
                .Where(h => h.HumanDoctorTypes.Any(hdt => service.ServiceDoctorTypes.Select(sdt => sdt.DoctorTypeId).Contains(hdt.DoctorTypeId)))
                .ToList();

            var model = new HtmlService
            {
                Service = service,
                Doctors = doctors,
                isEditor = IsAdmin()
            };

            return View("Item", model);
        }

        [HttpGet]
        public IActionResult List()
        {
            IQueryable<Service> services = context.Services;
            ViewBag.Admin = IsAdmin();
            return View("List", services.ToList());
        }

        [HttpGet]
        public IActionResult Add()
        {
            if (!IsAdmin()) return Forbid();

            var doctorTypes = context.DoctorTypes.ToList();
            var model = new HtmlService
            {
                Service = new Service(),
                DoctorTypes = doctorTypes,
                SelectedDoctorTypeIds = new List<int>()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(HtmlService model)
        {
            if (!IsAdmin()) return Forbid();

            var service = model.Service;
            // Добавляем выбранные типы врачей
            service.ServiceDoctorTypes = context.DoctorTypes
                .Where(dt => model.SelectedDoctorTypeIds.Contains(dt.Id))
                .Select(dt => new ServiceDoctorType { Service = service, DoctorType = dt })
                .ToList();

            context.Services.Add(service);
            context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            if (!IsAdmin()) return Forbid();

            var service = context.Services
                .Include(s => s.ServiceDoctorTypes).ThenInclude(sdt => sdt.DoctorType) // Включаем связанные типы врачей
                .FirstOrDefault(s => s.Id == id);

            if (service == null) return NotFound();

            var doctorTypes = context.DoctorTypes.ToList();
            var model = new HtmlService
            {
                Service = service,
                DoctorTypes = doctorTypes,
                SelectedDoctorTypeIds = service.ServiceDoctorTypes.Select(sdt => sdt.DoctorTypeId).ToList() // Выбираем типы, которые уже выбраны у услуги
            };

            return View("Update", model);
        }

        [HttpPost]
        public IActionResult Update(HtmlService model)
        {
            if (!IsAdmin()) return Forbid();

            var service = context.Services
                .Include(s => s.ServiceDoctorTypes)
                .FirstOrDefault(s => s.Id == model.Service.Id);

            if (service == null) return NotFound();

            // Обновляем поля услуги
            service.Name = model.Service.Name;
            service.Description = model.Service.Description;
            service.Price = model.Service.Price;
            service.IsActive = model.Service.IsActive;

            // Обновляем типы врачей
            service.ServiceDoctorTypes = context.DoctorTypes
                .Where(dt => model.SelectedDoctorTypeIds.Contains(dt.Id))
                .Select(dt => new ServiceDoctorType { Service = service, DoctorType = dt })
                .ToList();

            context.Services.Update(service);
            context.SaveChanges();

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Record(int doctor, int service)
        {
            var doctorEntity = context.Humans.FirstOrDefault(h => h.Id == doctor);
            var serviceEntity = context.Services.FirstOrDefault(s => s.Id == service);

            if (doctorEntity == null || serviceEntity == null)
            {
                return NotFound();
            }

            var availableTimeSlots = GetAvailableTimeSlotsForDoctor(doctor);

            var model = new ServiceRecordViewModel
            {
                Doctor = doctorEntity,
                Service = serviceEntity,
                AvailableTimeSlots = availableTimeSlots
            };

            return View("Record", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Record(int doctorId, int serviceId, string selectedTime)
        {
            var human = GetCurrentUser();
            if (human == null) return Forbid();

            var record = new ServiceList
            {
                DoctorId = doctorId,
                ServiceId = serviceId,
                ClientId = human.Id,
                DateTime = DateTime.Parse(selectedTime),
                Price = context.Services.FirstOrDefault(s => s.Id == serviceId)?.Price ?? 0
            };

            context.ServiceLists.Add(record);
            context.SaveChanges();

            return RedirectToAction("List");
        }

        private List<string> GetAvailableTimeSlotsForDoctor(int doctorId)
        {
            var availableSlots = new List<string>();
            var now = DateTime.Now;

            // Устанавливаем рабочие часы клиники: с 8 до 18 часов
            var clinicOpen = new TimeSpan(8, 0, 0);
            var clinicClose = new TimeSpan(18, 0, 0);

            // Определяем рабочие дни (с понедельника по пятницу) на неделю вперед
            for (int day = 0; day < 7; day++)
            {
                var currentDay = now.AddDays(day);

                if (currentDay.DayOfWeek == DayOfWeek.Saturday || currentDay.DayOfWeek == DayOfWeek.Sunday)
                    continue; // Пропускаем выходные

                for (int hour = clinicOpen.Hours; hour < clinicClose.Hours; hour++)
                {
                    var startTime = new DateTime(currentDay.Year, currentDay.Month, currentDay.Day, hour, 0, 0);

                    // Проверяем, не занят ли этот слот у данного доктора
                    bool isSlotTaken = context.ServiceLists
                        .Any(s => s.DoctorId == doctorId && s.DateTime == startTime);

                    if (!isSlotTaken)
                    {
                        availableSlots.Add(startTime.ToString("dd.MM.yyyy HH:mm"));
                    }
                }
            }

            return availableSlots;
        }
    }
}
