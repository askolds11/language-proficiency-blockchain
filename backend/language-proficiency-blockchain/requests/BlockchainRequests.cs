using System.ComponentModel.DataAnnotations;

namespace language_proficiency_blockchain.requests;

/// <summary>
/// Request to propose a new node to join the network. The proposer must provide
/// a public key (PEM), an address/endpoint, and a base64 signature proving control of the key.
/// </summary>
/// <param name="PublicKeyPem">Node public key in PEM format used to verify signatures.</param>
/// <param name="Address">Optional human-readable address or endpoint of the node.</param>
/// <param name="SignatureBase64">Base64-encoded signature over the proposal payload.</param>
/// <example>
/// {
///   "publicKeyPem": "-----BEGIN PUBLIC KEY-----...",
///   "address": "node-1.example.org:443",
///   "signatureBase64": "MEYCIQ..."
/// }
/// </example>
public record ProposeNodeRequest(
    [property: Required] string PublicKeyPem,
    [property: Required] string Address,
    [property: Required] string SignatureBase64
);

/// <summary>
/// Request for an existing approved node to approve another node.
/// </summary>
/// <param name="ApproverNodeId">Identifier of the node performing the approval.</param>
/// <param name="SignatureBase64">Base64-encoded signature from the approver.</param>
/// <example>
/// {
///   "approverNodeId": "5b1a2f2b-3f19-4c8c-8d33-8e6b9f9f1234",
///   "signatureBase64": "MEUCID..."
/// }
/// </example>
public record ApproveNodeRequest(
    [property: Required] Guid ApproverNodeId,
    [property: Required] string SignatureBase64
);

/// <summary>
/// Request to submit a language test result to be recorded on the blockchain.
/// </summary>
/// <param name="TestId">Identifier of the test (external system ID).</param>
/// <param name="StudentId">Identifier of the student (external system ID).</param>
/// <param name="InstitutionId">Identifier of the issuing institution.</param>
/// <param name="Score">Optional score or level (e.g., "C1", "95").</param>
/// <param name="SubmittedByNodeId">Identifier of the node submitting the result.</param>
/// <param name="Timestamp">UTC timestamp when the test was taken or recorded.</param>
/// <param name="SignatureBase64">Base64-encoded signature covering the request payload.</param>
/// <example>
/// {
///   "testId": "IELTS-2025-0001",
///   "studentId": "STU-12345",
///   "institutionId": "INST-999",
///   "score": "7.5",
///   "submittedByNodeId": "9d5e8c60-9f1f-4c84-9f7c-8f9d5e8c609f",
///   "timestamp": "2025-11-30T04:10:00Z",
///   "signatureBase64": "MEQCIF..."
/// }
/// </example>
public record SubmitResultRequest(
    [property: Required] Guid TestId,
    [property: Required] Guid StudentId,
    [property: Required] Guid InstitutionId,
    string? Score,
    [property: Required] Guid SubmittedByNodeId,
    [property: Required] DateTime Timestamp,
    [property: Required] string SignatureBase64
);
