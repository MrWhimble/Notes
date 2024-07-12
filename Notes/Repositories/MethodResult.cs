namespace Notes;

public struct MethodResult
{
    public bool Success { get; }
    public string Message { get; }

    public MethodResult(bool success, string message = "")
    {
        Success = success;
        Message = message;
    }

    public MethodResult Combine(MethodResult other)
    {
        return new MethodResult(Success && other.Success, $"{Message}{(string.IsNullOrWhiteSpace(Message) ? "" : "\n\r")}{other.Message}");
    }

    public override string ToString()
    {
        return $"Success: {Success}\n\r{Message}";
    }
}