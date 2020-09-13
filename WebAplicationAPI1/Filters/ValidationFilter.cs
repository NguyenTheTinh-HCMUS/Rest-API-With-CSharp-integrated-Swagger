using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAplicationAPI1.Contracts.V1.Responses;

namespace WebAplicationAPI1.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // before every  controller: 
            if (!context.ModelState.IsValid)
            {
                var errorInModelState= context.ModelState
                                                .Where(x => x.Value.Errors.Count > 0)
                                                .ToDictionary(kvp => kvp.Key,
                                                              kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray());
                var errorResopnse = new ErrorResponse { 
                Errors=new List<ErrorModel>()
                };
                foreach (var error in errorInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new ErrorModel { FieldName = error.Key, Message = subError };
                        errorResopnse.Errors.Add(errorModel);

                    }

                }
                context.Result = new BadRequestObjectResult(errorResopnse);
                return;
            }
            await next();
            // after evary controller
        }
    }
}
