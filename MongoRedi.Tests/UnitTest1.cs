using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRedi.Tests.MongoRediModels;
using MongoRedi.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MongoRedi.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly MongoDBRepository<User> _userRepository;

        static UnitTest1()
        {
            _userRepository = new MongoDBRepository<User>();
        }

        [TestMethod]
        public void Insert()
        {
            var newUser = new User
            {
                FirstName = "Harry",
                LastName = "Potter",
                Age = 10
            };

            var id = _userRepository.Insert(newUser);

            var insertedUser = _userRepository.GetById(id);

            Assert.IsNotNull(insertedUser);
        }

        [TestMethod]
        public void InsertMany()
        {
            var users =
                new List<User> {
                    new User
                    {
                        FirstName = "Hermione",
                        LastName = "Granger",
                        Age = 11
                    },  new User
                    {
                        FirstName = "Lord",
                        LastName = "Voldemort",
                        Age = 55
                    }, new User
                    {
                        FirstName = "Albus",
                        LastName = "Dumbledore",
                        Age = 150
                    }
                };

            _userRepository.InsertMany(users);

            var hermione = _userRepository.Search(x => x.FirstName == "Hermione" && x.LastName == "Granger").FirstOrDefault();
            var lord = _userRepository.Search(x => x.FirstName == "Lord" && x.LastName == "Voldemort").FirstOrDefault();
            var albus = _userRepository.Search(x => x.FirstName == "Albus" && x.LastName == "Dumbledore").FirstOrDefault();

            Assert.IsTrue(hermione.Age == 11);
            Assert.IsTrue(lord.Age == 55);
            Assert.IsTrue(albus.Age == 150);

            _userRepository.GetById(hermione.Id);
            _userRepository.GetById(lord.Id);
            _userRepository.GetById(albus.Id);
        }

        [TestMethod]
        public void Count()
        {
            var count = _userRepository.Count(x => x.Age == 11);

            Assert.IsTrue(count == 1);
        }

        [TestMethod]
        public void Update()
        {
            var user = _userRepository.GetAll().FirstOrDefault();

            var initalAge = user.Age;

            user.Age += 1;

            _userRepository.Update(user.Id, user);

            var updatedUser = _userRepository.GetById(user.Id);

            Assert.IsTrue(initalAge != updatedUser.Age);
        }

        [TestMethod]
        public void Delete()
        {
            var users = _userRepository.GetAll();

            foreach (var user in users)
            {
                _userRepository.Delete(user.Id);

                var deletedUser = _userRepository.GetById(user.Id);

                Assert.IsNull(deletedUser);
            }
        }
    }
}
