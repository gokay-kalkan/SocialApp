using BusinessLayer.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //'((Microsoft.AspNetCore.Http.DefaultHttpRequest)((Microsoft.AspNetCore.Http.DefaultHttpContext)context).Request).Form' threw an exception of type 'Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException'
                await _next(context); // Diğer middleware'lere ve sonrasında action'lara geçiş yapılır.
            }
            catch (AppException appEx) // Özel olarak tanımladığınız AppException'ı yakala.
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = appEx.StatusCode;

                var response = new
                {
                    statusCode = appEx.StatusCode,
                    message = appEx.Message,
                    errors = appEx.Errors //validation hataları
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }

            catch (Exception ex) // Genel hatalar için.
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    statusCode = context.Response.StatusCode,
                    message = "Internal Server Error from the custom middleware.",
                    detailedMessage = ex.Message // Development ortamında detaylı mesaj göndermek isterseniz.
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
            }
        }


      



    }
}
