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
                Age = 36
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
                        FirstName = "Harry",
                        LastName = "Potter",
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

            var kyle = _userRepository.Search(x => x.FirstName == "Harry" && x.LastName == "Potter").FirstOrDefault();
            var ed = _userRepository.Search(x => x.FirstName == "Lord" && x.LastName == "Voldemort").FirstOrDefault();
            var charlie = _userRepository.Search(x => x.FirstName == "Albus" && x.LastName == "Dumbledore").FirstOrDefault();

            Assert.IsTrue(kyle.Age == 11);
            Assert.IsTrue(ed.Age == 55);
            Assert.IsTrue(charlie.Age == 150);
        }

        [TestMethod]
        public void Count()
        {
            var count = _userRepository.Count(x => x.Age == 36);

            Assert.IsTrue(count == 2);
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
