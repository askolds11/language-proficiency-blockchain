using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.services;
using Microsoft.Extensions.DependencyInjection;

namespace language_proficiency_blockchain.Tests;

[NotInParallel]
public class InternalTests : BaseIntegrationTest
{
    [Test]
    public async Task AddStudent_adds_student()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var svc = scope.ServiceProvider.GetRequiredService<InternalService>();

        var studentId = Guid.NewGuid();
        await svc.AddStudent(studentId, "Alice", "Smith");

        var student = await db.Students.FindAsync(studentId);
        await Assert.That(student).IsNotNull();
        await Assert.That(student!.Name).IsEqualTo("Alice");
        await Assert.That(student.Surname).IsEqualTo("Smith");
    }

    [Test]
    public async Task AddStudent_throws_when_student_exists()
    {
        using var scope = Factory.Services.CreateScope();
        var svc = scope.ServiceProvider.GetRequiredService<InternalService>();

        var studentId = Guid.NewGuid();
        await svc.AddStudent(studentId, "Bob", "Jones");

        await Assert.That(() => svc.AddStudent(studentId, "Bob", "Jones"))
            .Throws<Exception>();
    }
}