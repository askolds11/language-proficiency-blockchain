using System.Security.Cryptography;
using language_proficiency_blockchain.Database;
using language_proficiency_blockchain.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace language_proficiency_blockchain.services;

internal class InternalService(
    AppDbContext dbContext
)
{
    public async Task AddInstitution(Guid id, string name, string address, string publicKeyPem)
    {
        var exists = await dbContext.Institutions.AnyAsync(x => x.Id == id);

        if (exists)
        {
            throw new Exception("Institution already exists");
        }
        
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        var publicKeyBytes = rsa.ExportRSAPublicKey();
        
        var institution = new InstitutionEntity
        {
            Id = id,
            BlockId = null,
            Address = address,
            PublicKeyPem = publicKeyBytes,
        };
        
        dbContext.Institutions.Add(institution);

        await dbContext.SaveChangesAsync();
    }

    public async Task AddStudent(Guid id, string? name, string? surname)
    {
        var exists = await dbContext.Students.AnyAsync(x => x.Id == id);
        if (exists)
        {
            throw new Exception("Student already exists");
        }

        var student = new StudentEntity
        {
            Id = id,
            Name = name,
            Surname = surname
        };

        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync();
    }
}