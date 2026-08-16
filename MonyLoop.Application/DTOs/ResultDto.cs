using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.DTOs
{
    public class ResultDto
    {
        public bool IsSuccess { get; set; }
        public bool IsFailure => !IsSuccess;
        public string? Error { get; set; }

    }

    public class ResultDto<T> : ResultDto
    {
        public T? Value { get; set; }

    }

}
