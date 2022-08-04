using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Reflection;

namespace LearningPlatformAPI.ActionFilters
{
    public class SampleActionFilter : Attribute , IAsyncActionFilter
    {
        private readonly DataContext dbContext;

        public SampleActionFilter(DataContext context)
        {
            this.dbContext = context;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue( "TOKEN", out var token))
            {
                var doesTokenExist = (from dt in dbContext.Person
                                     where dt.Token.ToString() == (string)token
                                     select dt).FirstOrDefault();

                if (doesTokenExist != null)
                {
                    await next();
                } 
            }
            context.Result = new UnauthorizedResult();
            return;
        }

        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    Console.ForegroundColor = ConsoleColor.Green;
        //    Console.WriteLine("This has run after the action");
        //}
    }
}
