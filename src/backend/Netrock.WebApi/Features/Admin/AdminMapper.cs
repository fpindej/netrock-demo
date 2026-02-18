using Netrock.Application.Features.Admin.Dtos;
using Netrock.WebApi.Features.Admin.Dtos;
using Netrock.WebApi.Features.Admin.Dtos.AssignRole;
using Netrock.WebApi.Features.Admin.Dtos.CreateUser;
using Netrock.WebApi.Features.Admin.Dtos.CreateRole;
using Netrock.WebApi.Features.Admin.Dtos.ListUsers;
using Netrock.WebApi.Features.Admin.Dtos.SetPermissions;
using Netrock.WebApi.Features.Admin.Dtos.UpdateRole;

namespace Netrock.WebApi.Features.Admin;

/// <summary>
/// Maps between admin Application layer DTOs and WebApi request/response DTOs.
/// </summary>
internal static class AdminMapper
{
    /// <summary>
    /// Returns a copy of the response with PII fields (email, username, phone) masked.
    /// Used in demo mode to prevent exposure of other users' personal information.
    /// </summary>
    public static AdminUserResponse WithAnonymizedPii(this AdminUserResponse response) => new()
    {
        Id = response.Id,
        Username = MaskEmail(response.Username),
        Email = MaskEmail(response.Email),
        FirstName = response.FirstName,
        LastName = response.LastName,
        PhoneNumber = response.PhoneNumber is not null ? "***" : null,
        Bio = response.Bio,
        AvatarUrl = response.AvatarUrl,
        Roles = response.Roles,
        EmailConfirmed = response.EmailConfirmed,
        LockoutEnabled = response.LockoutEnabled,
        LockoutEnd = response.LockoutEnd,
        AccessFailedCount = response.AccessFailedCount,
        IsLockedOut = response.IsLockedOut
    };

    /// <summary>
    /// Returns a copy of the list response with PII anonymized for all users except the caller.
    /// </summary>
    public static ListUsersResponse WithAnonymizedUsers(this ListUsersResponse response, Guid callerUserId) => new()
    {
        Items = response.Items.Select(u => u.Id == callerUserId ? u : u.WithAnonymizedPii()).ToList(),
        TotalCount = response.TotalCount,
        PageNumber = response.PageNumber,
        PageSize = response.PageSize
    };

    /// <summary>
    /// Masks an email address, preserving only the first character of the local part,
    /// the first character of the domain name, and the TLD.
    /// Example: <c>john.doe@gmail.com</c> → <c>j***@g***.com</c>
    /// </summary>
    private static string MaskEmail(string email)
    {
        var atIndex = email.IndexOf('@');
        if (atIndex < 1) return "***@***.com";

        var local = email[..atIndex];
        var domain = email[(atIndex + 1)..];
        var dotIndex = domain.LastIndexOf('.');

        if (dotIndex < 1)
            return $"{local[0]}***@***.com";

        var tld = domain[dotIndex..];
        return $"{local[0]}***@{domain[0]}***{tld}";
    }
    /// <summary>
    /// Maps an <see cref="AdminUserOutput"/> to an <see cref="AdminUserResponse"/>.
    /// </summary>
    public static AdminUserResponse ToResponse(this AdminUserOutput output) => new()
    {
        Id = output.Id,
        Username = output.UserName,
        Email = output.Email,
        FirstName = output.FirstName,
        LastName = output.LastName,
        PhoneNumber = output.PhoneNumber,
        Bio = output.Bio,
        AvatarUrl = output.AvatarUrl,
        Roles = output.Roles,
        EmailConfirmed = output.EmailConfirmed,
        LockoutEnabled = output.LockoutEnabled,
        LockoutEnd = output.LockoutEnd,
        AccessFailedCount = output.AccessFailedCount,
        IsLockedOut = output.IsLockedOut
    };

    /// <summary>
    /// Maps an <see cref="AdminUserListOutput"/> to a <see cref="ListUsersResponse"/>.
    /// </summary>
    public static ListUsersResponse ToResponse(this AdminUserListOutput output) => new()
    {
        Items = output.Users.Select(u => u.ToResponse()).ToList(),
        TotalCount = output.TotalCount,
        PageNumber = output.PageNumber,
        PageSize = output.PageSize
    };

    /// <summary>
    /// Maps an <see cref="AdminRoleOutput"/> to an <see cref="AdminRoleResponse"/>.
    /// </summary>
    public static AdminRoleResponse ToResponse(this AdminRoleOutput output) => new()
    {
        Id = output.Id,
        Name = output.Name,
        Description = output.Description,
        IsSystem = output.IsSystem,
        UserCount = output.UserCount,
        Permissions = output.Permissions
    };

    /// <summary>
    /// Maps an <see cref="AssignRoleRequest"/> to an <see cref="AssignRoleInput"/>.
    /// </summary>
    public static AssignRoleInput ToInput(this AssignRoleRequest request) => new(request.Role);

    /// <summary>
    /// Maps a <see cref="CreateRoleRequest"/> to a <see cref="CreateRoleInput"/>.
    /// </summary>
    public static CreateRoleInput ToInput(this CreateRoleRequest request) => new(request.Name, request.Description);

    /// <summary>
    /// Maps an <see cref="UpdateRoleRequest"/> to an <see cref="UpdateRoleInput"/>.
    /// </summary>
    public static UpdateRoleInput ToInput(this UpdateRoleRequest request) => new(request.Name, request.Description);

    /// <summary>
    /// Maps a <see cref="CreateUserRequest"/> to a <see cref="CreateUserInput"/>.
    /// </summary>
    public static CreateUserInput ToInput(this CreateUserRequest request) => new(request.Email, request.FirstName, request.LastName);

    /// <summary>
    /// Maps a <see cref="SetPermissionsRequest"/> to a <see cref="SetRolePermissionsInput"/>.
    /// </summary>
    public static SetRolePermissionsInput ToInput(this SetPermissionsRequest request) => new(request.Permissions);

    /// <summary>
    /// Maps a <see cref="RoleDetailOutput"/> to a <see cref="RoleDetailResponse"/>.
    /// </summary>
    public static RoleDetailResponse ToResponse(this RoleDetailOutput output) => new()
    {
        Id = output.Id,
        Name = output.Name,
        Description = output.Description,
        IsSystem = output.IsSystem,
        Permissions = output.Permissions,
        UserCount = output.UserCount
    };

    /// <summary>
    /// Maps a <see cref="PermissionGroupOutput"/> to a <see cref="PermissionGroupResponse"/>.
    /// </summary>
    public static PermissionGroupResponse ToResponse(this PermissionGroupOutput output) => new()
    {
        Category = output.Category,
        Permissions = output.Permissions
    };
}
