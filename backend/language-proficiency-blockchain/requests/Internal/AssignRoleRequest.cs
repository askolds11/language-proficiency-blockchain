using JetBrains.Annotations;
using language_proficiency_blockchain.Database.Models;

namespace language_proficiency_blockchain.requests.Internal;

/// <summary>
/// Request for assigning a role to a user.
/// </summary>
/// <param name="UserId">Id of the user</param>
/// <param name="Role">Role to assign</param>
[PublicAPI]
public sealed record AssignRoleRequest(
    Guid UserId,
    UserRole Role
);
