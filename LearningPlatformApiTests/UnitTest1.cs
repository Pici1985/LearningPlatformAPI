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
    }
}