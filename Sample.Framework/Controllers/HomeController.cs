using MongoDB.Bson;
using MongoRedi.Interfaces;
using MongoRedi.Repositories;
using Sample.Framework.Models;
using System.Web.Mvc;

namespace Sample.Framework.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMongoDBRepository<Student> _studentRepository;

        public HomeController()
        {
            _studentRepository = new MongoDBRepository<Student>();
        }

        public ActionResult Index()
        {
            var viewModel = _studentRepository.GetAll();

            return View(viewModel);
        }

        public ActionResult UpsertStudent(Student student)
        {
            var existingStudentId = Request.Form["Id"];

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

        public ActionResult DeleteStudent(string id)
        {
            _studentRepository.Delete(id);

            return RedirectToAction("Index");
        }
    }
}