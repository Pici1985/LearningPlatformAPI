using AutoFixture;
using LearningPlatformAPI.Controllers;
using LearningPlatformAPI.Data;
using LearningPlatformAPI.Data.Interfaces;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NuGet.Protocol;
using System.Diagnostics;

namespace LearningPlatformApiTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Assert_Test_Passes()
        {
            Assert.Pass();
        }
    }
    
    public class PersonControllerTests
    {
        private Mock<IPersonRepository>? _personRepository;
        private Fixture? _fixture;
        private PersonController? _controller;

        public PersonControllerTests() 
        { 
            _fixture = new Fixture();
            _personRepository = new Mock<IPersonRepository>();
        }

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Get_All_Persons_Returns_Persons_and_200()
        {
            //arrange
            var personList = _fixture.CreateMany<Person>(3).ToList();
            _personRepository.Setup(repo => repo.GetAllPersons()).Returns(personList);
            _controller = new PersonController(_personRepository.Object);

            //act
            var result = _controller.Get();
            var obj = result as ObjectResult;
            
            //assert
            
            Assert.IsNotNull(obj);

            Assert.That(obj.StatusCode, Is.EqualTo(200));

            Assert.That(personList.Count, Is.EqualTo(3));
        }
        
        [Test]
        public void Get_All_Persons_Throws_Exception()
        {
            //arrange
            _personRepository.Setup(repo => repo.GetAllPersons()).Throws(new Exception());
            _controller = new PersonController(_personRepository.Object);

            //act
            var result = _controller.Get();
            var obj = result as ObjectResult;
            
            //assert
            Assert.AreEqual(400, obj.StatusCode);
        }

        [Test]
        public void Post_Person_Returns_Person_and_200()
        {
            //arrange
            var person = _fixture.CreateMany<Person>(1).FirstOrDefault();

            _personRepository.Setup(repo => repo.PostPerson(person)).Returns(person);
            _controller = new PersonController(_personRepository.Object);

            //act

            var result = _controller.Post(person);
            var obj = result as ObjectResult;
            
            //assert            
            Assert.IsNotNull(obj);

            Assert.That(obj.StatusCode, Is.EqualTo(200));

        }
        
        [Test]
        public void Post_Person_ModelState_Invalid_Returns_400()
        {
            //arrange
            var person = _fixture.CreateMany<Person>(1).FirstOrDefault();
            _personRepository.Setup(repo => repo.PostPerson(person)).Returns(person);
            var controller = new PersonController(_personRepository.Object);
            controller.ModelState.AddModelError("test", "test");
            var modelstate = controller.ModelState;


            //act
            //ActionResult result = _controller.Index(new Person());

            var result = controller.Post(person);
            var obj = result as ObjectResult;
            
            //assert            
            Assert.IsTrue(!modelstate.IsValid);

            Assert.That(obj.StatusCode, Is.EqualTo(400));

            Assert.That(obj.Value, Has.Message.EqualTo("ModelState invalid"));

        }
        


        [Test]
        public void Post_Person_Returns_Throws_Exception()
        {
            //arrange
            var person = _fixture.CreateMany<Person>(1).FirstOrDefault();
            _personRepository.Setup(repo => repo.PostPerson(It.IsAny<Person>())).Throws(new Exception());
            _controller = new PersonController(_personRepository.Object);

            //act

            var result = _controller.Post(person);
            var obj = result as ObjectResult;
            
            //assert          
            Assert.That(obj.StatusCode, Is.EqualTo(400));
        }
    }
}