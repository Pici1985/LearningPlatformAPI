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
            dbContext = context;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue( "TOKEN", out var token))
            {
                var doesTokenExist = (from dt in dbContext.Person
                                     where dt.Token.ToString() == token
                                     select dt).FirstOrDefault();

                if (context.ActionArguments.TryGetValue("Id", out var value))
                {
                    var currentToken = (from ct in dbContext.Person
                                        where ct.UserId == (int)value
                                        select ct.Token).FirstOrDefault();

                    if (doesTokenExist?.Token == currentToken)
                    {
                        await next();
                    }
                    context.Result = new UnauthorizedResult();
                    return;
                }

                if (doesTokenExist != null)
                {           
                    await next();
                } 
            }
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}
