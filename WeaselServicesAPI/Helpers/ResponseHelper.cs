using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WeaselServicesAPI
{
    public static class ResponseHelper
    {
        public static JsonResult GenerateResponse(object? keys, int statusCode = 501, string contentType = "application/json")
        {
            return new JsonResult(keys)
            {
                StatusCode = (int?) statusCode,
                ContentType = contentType
            };
        }
    }
}
