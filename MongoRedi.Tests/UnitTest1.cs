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
                FirstName = "Kyle",
                LastName = "Barnes",
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
                        FirstName = "Kyle",
                        LastName = "Barnes",
                        Age = 36
                    },  new User
                    {
                        FirstName = "Ed",
                        LastName = "Piekarczyk",
                        Age = 25
                    }, new User
                    {
                        FirstName = "Charlie",
                        LastName = "Kocher",
                        Age = 65
                    }
                };

            _userRepository.InsertMany(users);

            var kyle = _userRepository.Search(x => x.FirstName == "Kyle" && x.LastName == "Barnes").FirstOrDefault();
            var ed = _userRepository.Search(x => x.FirstName == "Ed" && x.LastName == "Piekarczyk").FirstOrDefault();
            var charlie = _userRepository.Search(x => x.FirstName == "Charlie" && x.LastName == "Kocher").FirstOrDefault();

            Assert.IsTrue(kyle.Age == 36);
            Assert.IsTrue(ed.Age == 25);
            Assert.IsTrue(charlie.Age == 65);
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
