using GRA.Domain.Security;
using Microsoft.AspNetCore.Identity;

namespace GRA.Infra.Security;
internal sealed class SenhaHasherUsuario
{
}

public class PasswordHasherAdapter : IPasswordHasher
{
    private static readonly SenhaHasherUsuario _usuario = new();
    private readonly PasswordHasher<SenhaHasherUsuario> _hasher = new();

    public string Hash(string senha)
        => _hasher.HashPassword(_usuario, senha);

    public bool Verify(string senha, string senhaHash)
    {
        var resultado = _hasher.VerifyHashedPassword(_usuario, senhaHash, senha);

        return resultado is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}