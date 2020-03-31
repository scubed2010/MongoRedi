using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRedi.Tests.MongoRediModels;
using MongoRedi.Repositories;

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
    }
}
