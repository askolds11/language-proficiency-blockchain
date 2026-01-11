namespace language_proficiency_blockchain.Database.Models;

/// <summary>
/// Enumeration of user roles in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Student role - can view and submit test results.
    /// </summary>
    Student,

    /// <summary>
    /// Verificator role - can verify and validate test results.
    /// </summary>
    Verificator,

    /// <summary>
    /// Operator role - has full administrative access.
    /// </summary>
    Operator
}
