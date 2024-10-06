using MedicalClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;
using OfficeOpenXml;
using Microsoft.VisualBasic;

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

        private Human? GetCurrentUser()
        {
            var userId = httpContextAccessor.HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = context.Humans.Include(u => u.HumanDoctorTypes).ThenInclude(hdt => hdt.DoctorType).FirstOrDefault(i => i.Id == userId);
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
        public IActionResult Index(int id)
        {
            Human? currentUser = GetCurrentUser();
            ViewBag.Logout = false;
            if (id <= 0)
            {
                if (currentUser != null)
                {
                    var htmlUser = new HtmlHuman
                    {
                        Human = currentUser,
                        isEditor = true
                    };
                    ViewBag.Logout = true;
                    return View("Item", htmlUser);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else
            {
                var user = context.Humans.Include(u => u.HumanDoctorTypes).ThenInclude(hdt => hdt.DoctorType).FirstOrDefault(i => i.Id == id);
                if (user != null && ((currentUser != null && (currentUser.Type > 0 || user.Id == currentUser.Id)) || user.Type > 0))
                {
                    var htmlUser = new HtmlHuman
                    {
                        Human = user,
                        isEditor = currentUser != null && (currentUser.Id == id || currentUser.Type == 2)
                    };
                    if(currentUser != null && user.Id == currentUser.Id)
                    {
                        ViewBag.Logout = true;
                    }
                    return View("Item", htmlUser);
                }
            }
            return Forbid();
        }

        /*
         * 1 - услуги которые оказали
         * 2 - услуги которые окажут
         * 3 - услуги которые оказал
         * 4 - услуги которые окажет
         */
        [HttpGet]
        public IActionResult List(int client, int doctor, int type)
        {
            ViewBag.Client = client;
            ViewBag.Doctor = doctor;
            ViewBag.Type = type;
            var currentUser = GetCurrentUser();
            if (currentUser == null) return Forbid();

            // Получаем список всех услуг с включением связанных данных

            if (currentUser.Type > 0 || (client <= 0 || currentUser.Id == client))
            {
                client = currentUser.Id;
            }
            else
            {
                return Forbid();
            }

            List<ServiceList> list = Services(client, doctor, type);
            return list == null? BadRequest("Некорректный тип запроса") : View("List", list);
        }

        [HttpGet]
        public FileResult Report(int client, int doctor, int type)
        {
            var currentUser = GetCurrentUser();
            if (currentUser == null) return File(new byte[] { }, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet");

            // Получаем список всех услуг с включением связанных данных

            if (currentUser.Type > 0 || (client <= 0 || currentUser.Id == client))
            {
                client = currentUser.Id;
            }
            else
            {
                return File(new byte[] { }, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet");
            }
            List<ServiceList> list = Services(client, doctor, type);
            if(list == null)
            {
                return File(new byte[] { }, "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet");
            }
            
            // Путь к файлу с шаблоном
            string path = "/Reports/pattern.xlsx";
            //Путь к файлу с результатом
            string result = $"/Reports/pattern-{client}-{doctor}-{type}-{DateTime.Now.Ticks}.xlsx";
            FileInfo fi = new FileInfo(webHostEnvironment.WebRootPath + path);
            FileInfo fr = new FileInfo(webHostEnvironment.WebRootPath + result);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (ExcelPackage excelPackage = new ExcelPackage(fi))
            {
                //устанавливаем поля документа
                excelPackage.Workbook.Properties.Author = "SimpleMessenger";
                excelPackage.Workbook.Properties.Title = "Отношения";
                excelPackage.Workbook.Properties.Subject = "Список всех отношений пользователей проекта";
                excelPackage.Workbook.Properties.Created = DateTime.Now;
                //плучаем лист по имени.
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Interactions"];
                //получаем списко пользователей и в цикле заполняем лист данными
                int startLine = 2;
                

                foreach (ServiceList item in list)
                {
                    if (item != null)
                    {
                        worksheet.Cells[startLine, 1].Value = item.Doctor.Surname + " " + item.Doctor.Name + " " + item.Doctor.Patronymic;
                        worksheet.Cells[startLine, 2].Value = item.Service.Name;
                        worksheet.Cells[startLine, 3].Value = item.Price;
                        worksheet.Cells[startLine, 4].Value = item.Service.Price;
                        worksheet.Cells[startLine, 5].Value = item.DateTime.ToString("dd.MM.yyyy HH:mm");
                        startLine++;
                    }
                }
                //созраняем в новое место
                excelPackage.SaveAs(fr);
            }
            // Тип файла - content-type
            string file_type = "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
            // Имя файла - необязательно
            string file_name = "Report.xlsx";
            return File(result, file_type, file_name);
        }

        private List<ServiceList> Services(int client, int doctor, int type)
        {
            IQueryable<ServiceList> services = context.ServiceLists
                .Include(s => s.Service)
                .Include(d => d.Doctor)
                .Include(u => u.Client);
            // Получаем текущее время
            DateTime now = DateTime.Now;
            //Фильтруем по типу
            switch (type)
            {
                case 1: // Услуги, которые оказали (завершенные услуги клиенту)
                    services = services.Where(s => s.DateTime < now && s.ClientId == client);
                    break;
                case 2: // Услуги, которые окажут (предстоящие услуги клиенту)
                    services = services.Where(s => s.DateTime >= now && s.ClientId == client);
                    break;
                case 3: // Услуги, которые врач оказал (завершенные услуги врача)
                    services = services.Where(s => s.DateTime < now && s.DoctorId == doctor);
                    break;
                case 4: // Услуги, которые врач окажет (предстоящие услуги врача)
                    services = services.Where(s => s.DateTime >= now && s.DoctorId == doctor);
                    break;
                default:
                    return null;
            }
            return services.ToList();
        }






        [HttpGet]
        public IActionResult Login()
        {
            if (GetCurrentUser() != null)
            {
                return RedirectToAction("Index", new { id = GetCurrentUser().Id });
            }
            return View("Login");
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View("Registration");
        }

        [HttpPost]
        public IActionResult Login(Human human)
        {
            var existingUser = context.Humans
                .FirstOrDefault(h => h.Email == human.Email && h.Password == human.Password);
            if (existingUser != null)
            {
                httpContextAccessor.HttpContext.Session.SetInt32("UserId", existingUser.Id);
                return RedirectToAction("Index", new { id = existingUser.Id });
            }
            ModelState.AddModelError(string.Empty, "Неправильный логин или пароль");
            return View("Login", human);
        }

        [HttpPost]
        public IActionResult Registration(Human human)
        {
            if (context.Humans.Any(h => h.Email == human.Email))
            {
                ModelState.AddModelError(string.Empty, "Логин уже используется");
                return View("Registration", human);
            }

            context.Humans.Add(human);
            context.SaveChanges();
            httpContextAccessor.HttpContext.Session.SetInt32("UserId", human.Id);

            if (human.Id == 1) // Условие для админа (по вашему коду)
            {
                human.Type = 2;
                context.Humans.Update(human);
                context.SaveChanges();
            }
            return RedirectToAction("Index", new { id = human.Id });
        }

        [HttpGet]
        public IActionResult Logout()
        {
            httpContextAccessor.HttpContext.Session.Remove("UserId");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var currentUser = GetCurrentUser(); // Получаем текущего пользователя

            // Проверяем права доступа: если это не текущий пользователь и не администратор, отказать в доступе
            if ((currentUser == null || currentUser.Id != id) && !IsAdmin())
            {
                return Forbid(); // Отказ доступа, если пользователь не найден или нет прав
            }

            // Загружаем данные пользователя, которого мы редактируем
            var userToEdit = context.Humans
                .Include(u => u.HumanDoctorTypes).ThenInclude(hdt => hdt.DoctorType) // Включаем связанные типы врачей
                .FirstOrDefault(u => u.Id == id);

            if (userToEdit == null)
            {
                return NotFound(); // Возвращаем ошибку, если пользователь не найден
            }

            // Подготавливаем модель для представления
            var htmlUser = new HtmlHuman
            {
                Human = userToEdit, // Пользователь, которого редактируем
                DoctorTypes = context.DoctorTypes.ToList(), // Получаем все типы врачей для чекбоксов
                SelectedDoctorTypeIds = userToEdit.HumanDoctorTypes.Select(hdt => hdt.DoctorTypeId).ToList() // Выбираем типы, которые уже выбраны у пользователя
            };

            return View(htmlUser); // Возвращаем модель во вьюшку для редактирования
        }

        [HttpPost]
        public IActionResult Update(HtmlHuman model)
        {
            var currentUser = GetCurrentUser();

            // Проверяем права доступа: текущий пользователь или администратор может редактировать
            if ((currentUser == null || currentUser.Id != model.Human.Id) && !IsAdmin())
            {
                return Forbid();
            }

            // Загружаем пользователя из базы данных
            var userToUpdate = context.Humans
                .Include(u => u.HumanDoctorTypes) // Включаем связанные типы врачей
                .FirstOrDefault(u => u.Id == model.Human.Id);

            if (userToUpdate == null)
            {
                return NotFound(); // Возвращаем ошибку, если пользователь не найден
            }

            // Обновляем данные пользователя
            userToUpdate.Name = model.Human.Name;
            userToUpdate.Surname = model.Human.Surname;
            userToUpdate.Patronymic = model.Human.Patronymic;

            if (currentUser.Type == 2)   // Администратор может изменить тип
            {
                userToUpdate.Type = model.Human.Type;
            }

            // Обновляем описание, если это доктор
            if (userToUpdate.Type > 0)
            {
                userToUpdate.Description = model.Human.Description;

                // Обновляем типы врачей, если это доктор
                userToUpdate.HumanDoctorTypes = context.DoctorTypes
                    .Where(dt => model.SelectedDoctorTypeIds.Contains(dt.Id))
                    .Select(dt => new HumanDoctorType
                    {
                        HumanId = userToUpdate.Id,
                        DoctorTypeId = dt.Id
                    }).ToList();
            }

            // Сохраняем изменения в базе данных
            context.Humans.Update(userToUpdate);
            context.SaveChanges();

            return RedirectToAction("Index", new { id = model.Human.Id });
        }
    }
}
