using MedicalClinic.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

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

        [HttpGet]
        public IActionResult Index(int id)
        {
            Human? my = GetHuman();
            if (id <= 0)
            {
                if (my != null)
                {
                    //Добавить вывод профиля и список процедур
                }
            }
            else
            {
                Human? human = GetHuman();
                //Проверка существует ли человек
                //и проверка, является ли смотрящий админом, или запрашиваемый человек - доктор
                if (human != null && ((my != null && my.Type > 0) || human.Type == 1))
                {

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
            if (my != null)
            {
                if(my.Type > 0)
                {

                }
                else if (client <= 0 || my.Id == client)
                {
                    List<ServiceList> services = new List<ServiceList>();
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
            }
            return Forbid();
        }










    }
}
