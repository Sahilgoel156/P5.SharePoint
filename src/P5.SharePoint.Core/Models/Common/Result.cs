using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P5.SharePoint.Core.Models.Common
{
    public class Result
    {
        protected Result(bool succeeded, ResultError[] errors = default)
        {
            Succeeded = succeeded;
            if (errors != null && errors.Any())
            {
                _errors.AddRange(errors);
            }
        }
        public bool Succeeded { get; protected set; }

        private readonly List<ResultError> _errors = new List<ResultError>();
        public IEnumerable<ResultError> Errors => _errors;

        public static Result<T> Success<T>(T value) => new Result<T>(true, value);

        public static Result<T> Failed<T>(string code, string description) => Failed<T>(new ResultError(code, description));

        public static Result<T> Failed<T>(ResultError error)
        {
            var result = new Result<T>(false);
            if (error != null)
            {
                result._errors.Add(error);
            }

            return result;

        }
        public static Result<T> Failed<T>(IEnumerable<ResultError> errors) =>
            new Result<T>(false, default, errors as ResultError[] ?? errors.ToArray());

        public static Result<T> Failed<T>(params ResultError[] errors) => new Result<T>(false, default, errors);

        public override string ToString()
        {
            return Succeeded ? "Succeeded" : $"Failed: {string.Join(",", Errors.Select(x => x.Code))}";
        }
    }
    public class Result<T> : Result
    {
        public T Value { get; protected set; }

        protected internal Result(bool succeeded, T value = default, ResultError[] errors = default)
            : base(succeeded, errors)
        {
            Value = value;
        }
    }
}
