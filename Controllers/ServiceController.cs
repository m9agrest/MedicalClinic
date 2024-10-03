using MedicalClinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalClinic.Controllers
{
    public class ServiceController : Controller
    {
        private readonly MedicalClinicContext context;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ServiceController(
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
        private bool isAdmin()
        {
            Human? human = GetHuman();
            return human != null && human.Type == 2;
        }


        [HttpGet]
        public IActionResult List()
        {
            return View("List", context.Services
                .Where(s => true)
                .ToList());
        }


        [HttpGet]
        public IActionResult Add()
        {
            return isAdmin() ? View("Add") : Forbid();
        }
        [HttpPost]
        public IActionResult Add(Service service)
        {
            if (isAdmin())
            {
                context.Services.Add(service);
                context.SaveChanges();
                return Redirect("List");
            }
            return Forbid();
        }


        [HttpGet]
        public IActionResult Update(int id)
        {
            if(id > 0 && isAdmin())
            {
                Service? service = context.Services.FirstOrDefault(i => i.Id == id);
                if (service != null)
                {
                    return View("Update", service);
                }
                return NotFound();
            }
            return Forbid();
        }
        [HttpPost]
        public IActionResult Update(Service service)
        {
            if (isAdmin())
            {
                context.Services.Update(service);
                context.SaveChanges();
                return Redirect("List");
            }
            return Forbid();
        }

    }
}
