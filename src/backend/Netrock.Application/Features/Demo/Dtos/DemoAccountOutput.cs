namespace Netrock.Application.Features.Demo.Dtos;

/// <summary>
/// Output of a newly created demo account, containing the credentials needed for auto-login.
/// </summary>
/// <param name="UserId">The unique identifier of the created demo user.</param>
/// <param name="Email">The generated email address.</param>
/// <param name="Password">The generated password (used only for the internal login call).</param>
public record DemoAccountOutput(Guid UserId, string Email, string Password);
