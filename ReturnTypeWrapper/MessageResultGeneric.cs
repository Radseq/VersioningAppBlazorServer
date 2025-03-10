namespace ReturnTypeWrapper
{

    public class MessageResult<T>
    {
        //Preventing instantiation from constructor with private ctors
        //For simple positive flag

        private MessageResult()
        {
            IsSuccess = false;
        }
        private MessageResult(T t)
        {
            Value = t;
            IsSuccess = true;
        }

        private MessageResult(T t, string em)
        {
            Value = t;
            IsSuccess = false;
            ErrorData = new ErrorData(em);
        }

        //For errors
        private MessageResult(int ec, string em)
        {
            IsSuccess = false;
            ErrorData = new ErrorData(ec, em);
        }

        private MessageResult(ErrorData ed)
        {
            IsSuccess = false;
            ErrorData = ed;
        }

        public bool IsSuccess { get; private set; }

        public bool HasFailed => !IsSuccess;

        public ErrorData? ErrorData { get; private set; }

        public T? Value { get; set; }

        //public static MessageResult<T> Success()
        //{
        //    var res = new MessageResult<T>
        //    {
        //        IsSuccess = true
        //    };
        //    return res;
        //}

        public static MessageResult<T> Success(T t)
        {
            return new MessageResult<T>(t);
        }

        //public static MessageResult<T> Failure()
        //{
        //    var res = new MessageResult<T>
        //    {
        //        IsSuccess = false
        //    };
        //    return res;
        //}

        public static MessageResult<T> Failure(T t)
        {
            return new MessageResult<T>(t)
            {
                IsSuccess = false
            };
        }

        public static MessageResult<T> Failure(int errorCode, string errorMessage)
        {
            return new MessageResult<T>(errorCode, errorMessage);
        }

        public static MessageResult<T> PartialFailure(T t, string errorMessage)
        {
            return new MessageResult<T>(t, errorMessage);
        }

        public static MessageResult<T> Failure(ErrorData ed)
        {
            return new MessageResult<T>(ed);
        }

        public static MessageResult<T> Exception(Exception ex)
        {
            return new MessageResult<T>(new ErrorData(500, ex.Message));
        }

        public static MessageResult<T> FailureErrorNumberExtract(string errorMessage)
        {
            var res = errorMessage.Split(" ");
            var resCast = int.TryParse(res[0], out var intErrorValue);
            var errorCode = -1;//code number for undefined errors
            if (resCast) { errorCode = intErrorValue; }

            return new MessageResult<T>(errorCode, errorMessage);
        }

        public static MessageResult<T> FailureErrorNumberExtract(string errorMessage, string[] parms)
        {
            var errMsg = errorMessage;
            for (int i = 0; i < parms.Length; i++)
            {
                string placeholder = $"@{i}@";
                errMsg = errMsg.Replace(placeholder, parms[i]);
            }

            var res = errMsg.Split(" ");
            var resCast = int.TryParse(res[0], out var intErrorValue);
            var errorCode = -1;//code number for undefined errors
            if (resCast) { errorCode = intErrorValue; }
            return new MessageResult<T>(errorCode, errMsg);
        }

        public static MessageResult<T> FailureErrorNumberExtract(string errorMessage, string? parm = null)
        {
            var errMsg = errorMessage.Replace("@0@", parm ?? string.Empty);

            var res = errMsg.Split(" ");
            var resCast = int.TryParse(res[0], out var intErrorValue);
            var errorCode = -1;//code number for undefined errors
            if (resCast) { errorCode = intErrorValue; }
            return new MessageResult<T>(errorCode, errMsg);
        }
    }
}