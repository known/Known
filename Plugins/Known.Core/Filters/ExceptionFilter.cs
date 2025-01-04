﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace Known.Filters;

class ExceptionFilter : IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (!context.ExceptionHandled)
        {
            Console.WriteLine(context.Exception);
            var result = Result.Error(context.Exception.Message);
            context.Result = new JsonResult(result);
        }
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}