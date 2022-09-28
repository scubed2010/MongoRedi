using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRedi.Tests.MongoRediModels;
using MongoRedi.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MongoRedi.Tests
{
    [TestClass]
    public class SyncTest
    {
        private static readonly MongoDBRepository<Wizard> _wizardRepository;

        static SyncTest()
        {
            _wizardRepository = new MongoDBRepository<Wizard>();
        }

        [TestMethod]
        public void Insert()
        {
            var newUser = new Wizard
            {
                FirstName = "Harry",
                LastName = "Potter",
                Age = 10
            };

            var id = _wizardRepository.Insert(newUser);

            var insertedUser = _wizardRepository.GetById(id);

            Assert.IsNotNull(insertedUser);

            InsertMany();
        }

        public void InsertMany()
        {
            var users =
                new List<Wizard> {
                    new Wizard
                    {
                        FirstName = "Hermione",
                        LastName = "Granger",
                        Age = 11
                    },  new Wizard
                    {
                        FirstName = "Lord",
                        LastName = "Voldemort",
                        Age = 55
                    }, new Wizard
                    {
                        FirstName = "Albus",
                        LastName = "Dumbledore",
                        Age = 150
                    }
                };

            _wizardRepository.InsertMany(users);

            var hermione = _wizardRepository.Search(x => x.FirstName == "Hermione" && x.LastName == "Granger").FirstOrDefault();
            var lord = _wizardRepository.Search(x => x.FirstName == "Lord" && x.LastName == "Voldemort").FirstOrDefault();
            var albus = _wizardRepository.Search(x => x.FirstName == "Albus" && x.LastName == "Dumbledore").FirstOrDefault();

            Assert.IsTrue(hermione.Age == 11);
            Assert.IsTrue(lord.Age == 55);
            Assert.IsTrue(albus.Age == 150);

            _wizardRepository.GetById(hermione.Id);
            _wizardRepository.GetById(lord.Id);
            _wizardRepository.GetById(albus.Id);

            Count();
        }

        public void Count()
        {
            var count = _wizardRepository.Count(x => x.Age == 11);

            Assert.IsTrue(count == 1);

            Update();
        }

        public void Update()
        {
            var user = _wizardRepository.GetAll().FirstOrDefault();

            var initalAge = user.Age;

            user.Age += 1;

            _wizardRepository.Update(user.Id, user);

            var updatedUser = _wizardRepository.GetById(user.Id);

            Assert.IsTrue(initalAge != updatedUser.Age);

            Delete();
        }

        public void Delete()
        {
            var users = _wizardRepository.GetAll();

            foreach (var user in users)
            {
                _wizardRepository.Delete(user.Id);

                var deletedUser = _wizardRepository.GetById(user.Id);

                Assert.IsNull(deletedUser);
            }
        }
    }
}
