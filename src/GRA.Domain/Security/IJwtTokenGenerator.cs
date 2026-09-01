using System.Security.Claims;

namespace GRA.Domain.Security;

public interface IJwtTokenGenerator
{
    string GerarToken(IEnumerable<Claim> claims);
}