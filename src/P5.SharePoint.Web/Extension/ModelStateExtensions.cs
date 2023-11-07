using Microsoft.AspNetCore.Mvc.ModelBinding;
using P5.SharePoint.Core.Models.Common;
using System.Collections.Generic;
using System.Linq;

namespace P5.SharePoint.Web.Extension
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<ResultError> GetErrors(this ModelStateDictionary modelState)
        {
            return modelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => new ResultError(null, x.ErrorMessage))
                .ToArray();
        }
    }
}
