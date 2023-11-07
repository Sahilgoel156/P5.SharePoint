using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Models.Common
{
    public class ResultError
    {
        private const string DefaultCode = "Code undefined";
        public ResultError()
        {
        }

        public ResultError(string code, string description)
        {
            if (!string.IsNullOrEmpty(code))
            {
                Code = code;
            }
            Description = description;
        }

        public string Code { get; set; } = DefaultCode;

        public string Description { get; set; }
    }
}
