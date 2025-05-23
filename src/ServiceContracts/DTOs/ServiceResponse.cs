using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs
{
    public class ServiceResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public ServiceStatus StatusCode { get; set; } 
    }
    public enum ServiceStatus
    {
        Success,
        NotFound,
        BadRequest,
        Unauthorized,
        Conflict,
        InternalServerError,
        Forbidden
    }
}
