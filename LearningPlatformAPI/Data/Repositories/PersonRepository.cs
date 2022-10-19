using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LearningPlatformAPI.Enums;
using LearningPlatformAPI.Models;
using LearningPlatformAPI.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformAPI.Data;
using LearningPlatformAPI.ActionFilters;

namespace LearningPlatformAPI.Data.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DataContext _context;

        public PersonRepository(DataContext context)
        {
            _context = context;
        }

        //Methods for PersonController
        public List<Person> GetAllPersons()
        {
            var person = _context.Person.ToList();

            return person;
        }      

        public Person PostPerson(Person person)
        {
            _context.Person.Add(person);
            _context.SaveChanges();

            return person;
        }
        
        public async Task <Person> PutPerson(Person person)
        {
            var personToEdit = _context.Person.Where(x => x.UserId == person.UserId).FirstOrDefault();

            personToEdit.UserId = person.UserId;
            personToEdit.FirstName = person.FirstName;
            personToEdit.LastName = person.LastName;
            personToEdit.Email = person.Email;
            personToEdit.Password = person.Password;
            personToEdit.Gender = person.Gender;
            personToEdit.Age = person.Age;
            personToEdit.Occupation = person.Occupation;
            personToEdit.DateOfRegistration = person.DateOfRegistration;
            personToEdit.Token = person.Token;  

            _context.SaveChanges();

            return personToEdit;
        }     

        public async Task<bool> DeletePerson(int id)
        {
            var person = _context.Person.Where(x => x.UserId == id).FirstOrDefault();

            if (person != null) 
            { 
                _context.Person.Remove(person);
                _context.SaveChanges();
        
                return true;
            }
            return false; 
        }       
    }
}

