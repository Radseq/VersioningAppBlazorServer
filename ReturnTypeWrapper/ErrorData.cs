namespace ReturnTypeWrapper;

public class ErrorData
{
    public int Code { get; set; } = -1;
    public string Description { get; set; } = "FIXME_NoErrorDescription";

    public List<ErrorData> ExtendedErrors { get; set; } = [];

    public ErrorData()
    {
    }

    public ErrorData(string desc)
    {
        Description = desc;
    }

    public ErrorData(int code, string desc)
    {
        Code = code;
        Description = desc;
    }

    public ErrorData(ErrorData errorData)
    {
        ExtendedErrors.Add(errorData);
    }

    public ErrorData(List<ErrorData> errorDatas)
    {
        ExtendedErrors = [.. ExtendedErrors, .. errorDatas];
    }

    public ErrorData(int code, string desc, ErrorData errorData)
    {
        Code = code;
        Description = desc;
        ExtendedErrors.Add(errorData);
    }

    public ErrorData(int code, string desc, List<ErrorData> errorDatas)
    {
        Code = code;
        Description = desc;
        ExtendedErrors = [.. ExtendedErrors, .. errorDatas];
    }
}
