using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Common
{
    public class Result
    {
        protected readonly List<Error> _errors = [];
        public bool IsSuccess => _errors.Count == 0;
        public bool Failure => !IsSuccess;

        public IReadOnlyList<Error> Errors => _errors;

        // OK Success
        protected Result() { }

        // Fail with Error (one)
        protected Result(Error error)
        {
            _errors.Add(error);
        }

        // fail with Errors (many)

        protected Result(List<Error> errors)
        {
            _errors.AddRange(errors);
        }

        public static Result Ok() => new Result();
        public static Result Fail(Error error) => new Result(error);
        public static Result Fail(List<Error> errors) => new Result(errors);

    }


    public class Result<TValue> : Result
    {
        private readonly TValue _value;

        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot Access The Value Of Failed Result");

        // OK - Success with value

        private Result(TValue value)
        {
            _value = value;
        }

        // Fail - Fail with Error 
        private Result(Error error) : base(error)
        {
            _value = default!;
        }
        //Fail - fail with Errors
        private Result(List<Error> errors) : base(errors)
        {
            _value = default!;
        }

        public static Result<TValue> Ok(TValue value)
        => new Result<TValue>(value);

        public static new Result<TValue> Fail(Error error)
            => new Result<TValue>(error);

        // ممكن اشيل Result<TValue > 
        // عادي عشان هو كدا كدا متعرف في عنولن الميثود 
        public static new Result<TValue> Fail(List<Error> errors)
            => new(errors);


        public static implicit operator Result<TValue>(TValue value)
            => Ok(value);

        public static implicit operator Result<TValue>(Error error)
            => Fail(error);

        public static implicit operator Result<TValue>(List<Error> errors)
        => Fail(errors);

    }
}
