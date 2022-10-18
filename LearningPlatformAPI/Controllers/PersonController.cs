using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LearningPlatformAPI.Data;
using LearningPlatformAPI.Models;
using LearningPlatformAPI.ActionFilters;
using LearningPlatformAPI.Data.Interfaces;
using System.Net.WebSockets;
using System.Diagnostics;

namespace LearningPlatformAPI.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonRepository _personRepo;

        public PersonController(IPersonRepository personRepository)
        {
            _personRepo = personRepository;
        }

        [HttpGet("get")]
        public ActionResult Get()
        {
            try 
            { 
                var person = _personRepo.GetAllPersons();
                
                return Ok(person);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("post")]
        public ActionResult Post([FromBody]Person person)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _personRepo.PostPerson(person);
                    return StatusCode(200, result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex) 
            {
                return BadRequest(ex);
            }
        }

        // how to do this best??? 
        [HttpPut("put")]
        public ActionResult Put([FromBody] Person person)
        {
            try
            {
                var result = _personRepo.PutPerson(person);

                return StatusCode(200, result);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex);
            } 
        }

                
        [HttpDelete("delete")]
        public async Task <IActionResult> Delete(int id)
        {
            try
            {
                var result = _personRepo.DeletePerson(id);

                return StatusCode(200, result);
            }
            catch (Exception ex) 
            { 
                return BadRequest(ex);
            }
        }

        
    }
}
