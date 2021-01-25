using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRedi.Tests.MongoRediModels;
using MongoRedi.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace MongoRedi.Tests
{
    [TestClass]
    public class AsyncTest
    {
        private static readonly MongoDBRepository<User> _userRepository = new MongoDBRepository<User>();

        [TestMethod]
        public void RunAsyncTest()
        {
            InsertAsync();
        }

        public async void InsertAsync()
        {
            var newUser = new User
            {
                FirstName = "Harry",
                LastName = "Potter",
                Age = 10
            };

            var id = await _userRepository.InsertAsync(newUser);

            var insertedUser = await _userRepository.GetByIdAsync(id);

            Assert.IsNotNull(insertedUser);

            InsertManyAsync();
        }

        public async void InsertManyAsync()
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

            await _userRepository.InsertManyAsync(users);

            var h = await _userRepository.SearchAsync(x => x.FirstName == "Hermione" && x.LastName == "Granger");
            var hermione = h.FirstOrDefault();

            var l = await _userRepository.SearchAsync(x => x.FirstName == "Lord" && x.LastName == "Voldemort");
            var lord = l.FirstOrDefault();

            var a = await _userRepository.SearchAsync(x => x.FirstName == "Albus" && x.LastName == "Dumbledore");
            var albus = a.FirstOrDefault();

            Assert.IsTrue(hermione.Age == 11);
            Assert.IsTrue(lord.Age == 55);
            Assert.IsTrue(albus.Age == 150);

            await _userRepository.GetByIdAsync(hermione.Id);
            await _userRepository.GetByIdAsync(lord.Id);
            await _userRepository.GetByIdAsync(albus.Id);

            CountAsync();
        }

        public async void CountAsync()
        {
            var count = await _userRepository.CountAsync(x => x.Age == 11);

            Assert.IsTrue(count == 1);

            UpdateAsync();
        }

        public async void UpdateAsync()
        {
            var u = await _userRepository.GetAllAsync();
            var user = u.FirstOrDefault();

            var initalAge = user.Age;

            user.Age += 1;

            await _userRepository.UpdateAsync(user.Id, user);

            var updatedUser = await _userRepository.GetByIdAsync(user.Id);

            Assert.IsTrue(initalAge != updatedUser.Age);

            DeleteAsync();
        }

        public async void DeleteAsync()
        {
            var users = await _userRepository.GetAllAsync();

            foreach (var user in users)
            {
                await _userRepository.DeleteAsync(user.Id);

                var deletedUser = await _userRepository.GetByIdAsync(user.Id);

                Assert.IsNull(deletedUser);
            }
        }
    }
}
