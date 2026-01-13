using language_proficiency_blockchain.Database.Models;
using Microsoft.AspNetCore.Authorization;

namespace language_proficiency_blockchain.Authorization;

/// <summary>
/// Authorization policies for role-based access control.
/// </summary>
internal static class AuthorizationPolicies
{
    /// <summary>
    /// Policy name for Student role.
    /// </summary>
    public const string StudentOnly = nameof(StudentOnly);

    /// <summary>
    /// Policy name for Verificator role.
    /// </summary>
    public const string VerificatorOnly = nameof(VerificatorOnly);

    /// <summary>
    /// Policy name for Operator role.
    /// </summary>
    public const string OperatorOnly = nameof(OperatorOnly);

    /// <summary>
    /// Policy name for Verificator or Operator roles.
    /// </summary>
    public const string VerificatorOrOperator = nameof(VerificatorOrOperator);

    /// <summary>
    /// Policy name for Everyone (no restrictions).
    /// </summary>
    public const string Everyone = nameof(Everyone);

    /// <summary>
    /// Registers all authorization policies.
    /// </summary>
    /// <param name="services">Service collection.</param>
    public static void AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(StudentOnly, policy =>
                policy.RequireRole(UserRole.Student.ToString()));

            options.AddPolicy(VerificatorOnly, policy =>
                policy.RequireRole(UserRole.Verificator.ToString()));

            options.AddPolicy(OperatorOnly, policy =>
                policy.RequireRole(UserRole.Operator.ToString()));

            options.AddPolicy(Everyone, policy => 
                policy.RequireAuthenticatedUser());

            options.AddPolicy(VerificatorOrOperator, policy =>
                policy.RequireRole(
                    UserRole.Verificator.ToString(),
                    UserRole.Operator.ToString()));
        });
    }
}
