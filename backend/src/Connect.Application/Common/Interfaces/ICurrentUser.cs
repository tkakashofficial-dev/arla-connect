namespace Connect.Application.Common.Interfaces;

/// <summary>
/// The authenticated caller, resolved from the JWT. Lets the Application layer
/// scope data to the current user/customer without touching HttpContext directly.
/// </summary>
public interface ICurrentUser
{
    Guid? UserId { get; }
    Guid? BusinessCustomerId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
