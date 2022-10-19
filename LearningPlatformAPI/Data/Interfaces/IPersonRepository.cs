using LearningPlatformAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;


namespace LearningPlatformAPI.Data.Interfaces
{
    public interface IPersonRepository 
    {
        List<Person> GetAllPersons();
               
        Person PostPerson(Person person);
        Task <Person> PutPerson(Person person);

        Task <bool> DeletePerson(int id);
    }
}
