using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Models.Common
{
    [Serializable]
    public class ResultException : ApplicationException
    {
        public static ResultException Create(Result result) => new ResultException(result);

        public ResultException(Result result)
            : this(null, null, result)
        {
        }

        public ResultException(string message, Result result)
            : this(message, null, result)
        {
        }

        public ResultException(string message, Exception innerException, Result result)
            : base(message, innerException)
        {
            Errors = result.Errors;
        }

        protected ResultException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public IEnumerable<ResultError> Errors { get; protected set; }
    }
}
