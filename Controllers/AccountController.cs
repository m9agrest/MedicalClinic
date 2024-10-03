using MedicalClinic.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MedicalClinic.Controllers
{
    public class AccountController : Controller
    {
        private readonly MedicalClinicContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(
            MedicalClinicContext context,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
            this.webHostEnvironment = webHostEnvironment;
        }
        private Human? GetHuman()
        {
            var userId = httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                Human human = context.Humans.FirstOrDefault(i => i.Id == userId);
                if (human != null)
                {
                    return human;
                }
                httpContextAccessor.HttpContext.Session.Remove("UserId");
            }
            return null;
        }
        private Human? GetHuman(int id)
        {
            return context.Humans.FirstOrDefault(i => i.Id == id);
        }
        private Doctor? GetDoctor(int id)
        {
            return context.Doctors.FirstOrDefault(i => i.HumanId == id);
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            Human? my = GetHuman();
            if (id <= 0)
            {
                if (my != null)
                {
                    HtmlHuman html = new HtmlHuman();
                    html.Client = my;
                    html.Doctor = GetDoctor(my.Id);
                    return View("Item", html);
                }
            }
            else
            {
                Human? human = GetHuman(id);
                //Проверка существует ли человек
                //и проверка, является ли смотрящий админом, или запрашиваемый человек - доктор
                if (human != null && ((my != null && my.Type > 0) || human.Type == 1))
                {
                    HtmlHuman html = new HtmlHuman();
                    html.Client = human;
                    html.Doctor = GetDoctor(id);
                    return View("Item", html);
                }
            }
            //Ошибка доступа
            return Forbid();
        }

        /*
         * type:
         * 1 - оказанные мне услуги
         * 2 - предстоящие услуги
         * 3 - услуги которые я оказал
         * 4 - услуги которые я окажу
         */
        [HttpGet]
        public IActionResult List(int client, int doctor, int type)
        {
            Human? my = GetHuman();
            List<ServiceList>? services = null;
            if (my != null)
            {
                if (my.Type > 0)
                {
                    if (client > 0)
                    {
                        if (doctor > 0)
                        {
                            services = context.ServiceLists
                                .Where(s => s.DoctorId == doctor && s.ClientId == client)
                                .Include(s => s.Service)
                                .ToList();
                        }
                        else
                        {
                            services = context.ServiceLists
                                .Where(s => s.ClientId == client)
                                .Include(s => s.Service)
                                .ToList();
                        }
                    }
                    else
                    {
                        if(doctor > 0)
                        {
                            services = context.ServiceLists
                                .Where(s => s.DoctorId == doctor)
                                .Include(s => s.Service)
                                .ToList();
                        }
                    }
                }
                else if (client <= 0 || my.Id == client)
                {
                    if(type > 2 && doctor > 0)
                    {
                        services = context.ServiceLists
                            .Where(s => s.DoctorId == doctor && s.ClientId == my.Id)
                            .Include(s => s.Service)
                            .ToList();
                    }
                    else
                    {
                        services = context.ServiceLists
                            .Where(s => s.ClientId == my.Id)
                            .Include(s => s.Service)
                            .ToList();
                    }
                }
                else
                {
                    return Forbid();
                }
                if (services != null)
                {
                    return View("List", services);
                }
                return NotFound();
            }
            return Forbid();
        }
    }
}
