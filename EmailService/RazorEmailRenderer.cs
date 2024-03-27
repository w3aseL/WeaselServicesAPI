using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class RazorEmailRenderer : IEmailRenderer
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorEmailRenderer(
            IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> Render<T>(T model)
        {
            // Note: You can also support multiple languages by separating each locale into a folder
            var viewPath = $"~/EmailTemplates/{typeof(T).Name}.cshtml";
            var result = _razorViewEngine.GetView(null, viewPath, true);

            if (result.Success != true)
            {
                var searchedLocations = string.Join("\n", result.SearchedLocations);
                throw new InvalidOperationException($"Could not find this view: {viewPath}. Searched locations:\n{searchedLocations}");
            }

            var view = result.View;

            var httpContext = new DefaultHttpContext();
            httpContext.RequestServices = _serviceProvider;

            var actionContext = new ActionContext(
                    httpContext,
                    httpContext.GetRouteData(),
                    new ActionDescriptor()
                );

            using (var writer = new StringWriter())
            {
                var viewDataDict = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary());

                viewDataDict.Model = model;

                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    viewDataDict,
                    new TempDataDictionary(
                        httpContext.HttpContext,
                        _tempDataProvider
                    ),
                    writer,
                    new HtmlHelperOptions { }
                );

                await view.RenderAsync(viewContext);

                return writer.ToString();
            }
        }
    }
}
