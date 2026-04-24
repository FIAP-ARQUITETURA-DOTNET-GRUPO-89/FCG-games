using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FgcGames.Infra.Services;
using FgcGames.Shared.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shouldly;

namespace FgcGames.UnitTests.Infra.Services;

public class TokenServiceTests
{
    private readonly JwtSettings _jwtSettings;
    private readonly TokenService _sut;

    public TokenServiceTests()
    {
        _jwtSettings = new JwtSettings
        {
            SecurityKey = "super-secret-key-123456789-very-secure-key",
            Issuer = "fgc-games",
            ExpirationHours = 2
        };

        var options = Options.Create(_jwtSettings);

        _sut = new TokenService(options);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_GerarToken_Entao_RetornaTokenValido()
    {
        // Arrange
        var email = "user@fgc.com";
        var role = "Admin";

        // Act
        var token = _sut.GenerateJwtToken(email, role);

        // Assert
        token.ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Dado_TokenGerado_Quando_Decodificar_Entao_ContemClaimsCorretas()
    {
        // Arrange
        var email = "user@fgc.com";
        var role = "Admin";

        // Act
        var token = _sut.GenerateJwtToken(email, role);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Assert
        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value
            .ShouldBe(email);

        jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value
            .ShouldBe(role);

        jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value
            .ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Dado_TokenGerado_Quando_ValidarAssinatura_Entao_EValido()
    {
        // Arrange
        var token = _sut.GenerateJwtToken("user@fgc.com", "Admin");

        var tokenHandler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,

            ValidateAudience = false,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Act
        var principal = tokenHandler.ValidateToken(token, parameters, out var validatedToken);

        // Assert
        principal.ShouldNotBeNull();
        validatedToken.ShouldBeOfType<JwtSecurityToken>();
    }

    [Fact]
    public void Dado_TokenGerado_Quando_VerificarExpiracao_Entao_RespeitaConfiguracao()
    {
        // Arrange
        var agora = DateTime.UtcNow;

        // Act
        var token = _sut.GenerateJwtToken("user@fgc.com", "Admin");

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        // Assert
        var expiracaoEsperada = agora.AddHours(_jwtSettings.ExpirationHours);

        jwt.ValidTo.ShouldBeInRange(
            expiracaoEsperada.AddSeconds(-5),
            expiracaoEsperada.AddSeconds(5));
    }

    [Fact]
    public void Dado_GeracoesDiferentes_Quando_GerarToken_Entao_JtiDeveSerUnico()
    {
        // Arrange
        var email = "user@fgc.com";
        var role = "Admin";

        var handler = new JwtSecurityTokenHandler();

        // Act
        var token1 = handler.ReadJwtToken(_sut.GenerateJwtToken(email, role));
        var token2 = handler.ReadJwtToken(_sut.GenerateJwtToken(email, role));

        var jti1 = token1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        var jti2 = token2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

        // Assert
        jti1.ShouldNotBe(jti2);
    }
}
