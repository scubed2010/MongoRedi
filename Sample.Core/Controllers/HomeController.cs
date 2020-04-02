using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoRedi.Interfaces;
using Sample.Core.Models;

namespace Sample.Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoDBRepository<Student> _studentRepository;


        public HomeController(
            ILogger<HomeController> logger,
            IMongoDBRepository<Student> studentRepository
            )
        {
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public IActionResult Index()
        {
            var viewModel = _studentRepository.GetAll();

            return View(viewModel);
        }

        public IActionResult UpsertStudent(Student student)
        {
            var existingStudentId = Request.Form["Id"].ToString();

            if (!string.IsNullOrEmpty(existingStudentId))
            {
                student.Id = ObjectId.Parse(existingStudentId);
                _studentRepository.Update(student.Id, student);
            }
            else
            {
                _studentRepository.Insert(student);
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteStudent(string id)
        {
            _studentRepository.Delete(id);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
