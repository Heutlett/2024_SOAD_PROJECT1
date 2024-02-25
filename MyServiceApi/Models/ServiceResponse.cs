using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServiceApi.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }

        public bool Success { get; set; } = true;

        public string Message { get; set; } = string.Empty;

        public void SetError(string errorMessage)
        {
            Success = false;
            Message = errorMessage;
        }
        public void SetSuccess(string? successMessage, object? data = null)
        {
            Message = successMessage ?? "";
            Data = (T?)data;
        }
    }
}