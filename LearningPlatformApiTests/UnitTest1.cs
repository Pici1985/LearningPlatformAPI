using AutoFixture;
using LearningPlatformAPI.Controllers;
using LearningPlatformAPI.Data;
using LearningPlatformAPI.Data.Interfaces;
using LearningPlatformAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
        public void Test1()
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
        public void Get_All_Persons_Returns_Persons()
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
        }
    }
}