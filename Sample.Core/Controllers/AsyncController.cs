using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoRedi.Interfaces;
using Sample.Core.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Sample.Core.Controllers
{
    public class AsyncController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMongoDBRepository<Student> _studentRepository;


        public AsyncController(
            ILogger<HomeController> logger,
            IMongoDBRepository<Student> studentRepository
            )
        {
            _logger = logger;
            _studentRepository = studentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _studentRepository.GetAllAsync();

            return View(viewModel);
        }

        public async Task<IActionResult> UpsertStudent(Student student)
        {
            var existingStudentId = Request.Form["Id"].ToString();

            if (!string.IsNullOrEmpty(existingStudentId))
            {
                student.Id = ObjectId.Parse(existingStudentId);
                await _studentRepository.UpdateAsync(student.Id, student);
            }
            else
            {
                await _studentRepository.InsertAsync(student);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteStudent(string id)
        {
            await _studentRepository.DeleteAsync(id);

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
