using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyContactBook.Model.Domains
{
    public class Response<T>
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public T Data { get; set; }

        public Response<T> Success(string message, int statusCode, T data)
        {
            return new Response<T>
            {
                Message = message,
                StatusCode = statusCode,
                Data = data
            };
        } 
        
        public Response<T> Failed(string message, int statusCode)
        {
            return new Response<T>
            {
                Message = message,
                StatusCode = statusCode,
            };
        }





    }
}
