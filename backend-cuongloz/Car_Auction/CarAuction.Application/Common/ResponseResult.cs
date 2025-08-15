namespace CarAuction.Application.Common
{
    public class ResponseResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public static ResponseResult<T> SuccessResult(T data, string message = "")
        {
            return new ResponseResult<T> { Success = true, Data = data, Message = message };
        }

        public static ResponseResult<T> FailResult(string message)
        {
            return new ResponseResult<T> { Success = false, Message = message };
        }
    }
}
