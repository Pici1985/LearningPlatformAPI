using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Reflection;
using System.Security.Policy;

namespace LearningPlatformAPI.ActionFilters
{
    public class IdActionFilter : Attribute, IAsyncActionFilter
    {
        private readonly DataContext dbContext;

        public IdActionFilter(DataContext context)
        {
            this.dbContext = context;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("TOKEN", out var token))
            {
                var doesTokenExist = (from dt in dbContext.Person
                                      where dt.Token.ToString() == (string)token
                                      select dt).FirstOrDefault();
                                
                //this returns a bool
                var currentId = context.ActionArguments.TryGetValue("Id", out var value);
               
                var currentToken = (from ct in dbContext.Person
                                    where ct.UserId == (int)value
                                    select ct.Token).FirstOrDefault();


                if (doesTokenExist.Token == currentToken)
                {
                    await next();
                }
            }

            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
