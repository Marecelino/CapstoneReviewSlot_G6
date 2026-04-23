namespace Registration.Application.Ultils;

public static class ErrorHelper
{
    public static Exception WithStatus(int statusCode, string message)
    {
        var ex = new Exception(message);
        ex.Data["StatusCode"] = statusCode;
        return ex;
    }

    public static Exception NotFound(string message = "Not found.")
        => WithStatus(404, message);

    public static Exception BadRequest(string message = "Invalid data.")
        => WithStatus(400, message);

    public static Exception Conflict(string message = "Conflict.")
        => WithStatus(409, message);

    public static Exception Forbidden(string message = "Forbidden.")
        => WithStatus(403, message);

    public static Exception Internal(string message = "Internal error.")
        => WithStatus(500, message);
}
