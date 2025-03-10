namespace ReturnTypeWrapper;

public class MessageResult
{
    //Preventing instantiation from constructor with private ctors
    //For simple positive flag
    private MessageResult()
    {
        IsSuccess = true;
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

    public ErrorData? ErrorData { get; private set; } = null;

    public static MessageResult Success()
    {
        return new MessageResult();
    }

    public static MessageResult Failure(int errorCode, string errorMessage)
    {
        return new MessageResult(errorCode, errorMessage);
    }

    public static MessageResult Exception(Exception ex)
    {
        return new MessageResult(new ErrorData(500, ex.Message));
    }

    public static MessageResult Failure(ErrorData ed)
    {
        return new MessageResult(ed);
    }
    public static MessageResult FailureErrorNumberExtract(string errorMessage)
    {
        var res = errorMessage.Split(" ");
        var resCast = int.TryParse(res[0], out var intErrorValue);
        var errorCode = -1;//code number for undefined errors
        if (resCast) { errorCode = intErrorValue; }
        return new MessageResult(errorCode, errorMessage);
    }

    public static MessageResult FailureErrorNumberExtract(string errorMessage, string[] parms)
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
        return new MessageResult(errorCode, errMsg);
    }

    public static MessageResult FailureErrorNumberExtract(string errorMessage, string? parm = null)
    {
        var errMsg = errorMessage.Replace("@0@", parm ?? string.Empty);

        var res = errMsg.Split(" ");
        var resCast = int.TryParse(res[0], out var intErrorValue);
        var errorCode = -1;//code number for undefined errors
        if (resCast) { errorCode = intErrorValue; }
        return new MessageResult(errorCode, errMsg);
    }
}