using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Api.Models.Options;
using Api.Services.Crypt;
using Apis.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.DAL;
using Api.Utils;

namespace Api.Services.Auth
{
	public interface IJwtService
	{
		string GenerateRefreshToken();
        string GenereteJwtToken(string userName, IdentityUser user, string role);
        string GenereteEmailToken(User user);
        string DecryptEmailToken(string token);
    }
    
    //сервис предоставление JWT-токенов на основе JWE->токен не только подписывается сервером, но еще и шифруется. 
	public class JwtService : IJwtService
	{
        private readonly IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions;
        private readonly IOptions<AuthOptions> authOptions;


        private readonly IJwtSigningEncodingKey signInEncodingKey;
        private readonly IJwtEncryptingEncodingKey jwtEncryptionEncodingKey;
        private readonly IAesCipher crypt;

        public JwtService(IOptions<TokenLifeTimeOptions> tokenLifeTimeOptions,
            IOptions<AuthOptions> authOptions,
            IJwtSigningEncodingKey signInEncodingKey,
            IJwtEncryptingEncodingKey jwtEncryptionEncodingKey,
            IAesCipher crypt)
        {
            this.tokenLifeTimeOptions = tokenLifeTimeOptions;
            this.authOptions = authOptions;

            this.signInEncodingKey = signInEncodingKey;
            this.jwtEncryptionEncodingKey = jwtEncryptionEncodingKey;
            this.crypt = crypt;
        }

		public string GenerateRefreshToken()
		{
            return CommonMethods.GenerateRandomString(32);
		}

        public string GenereteJwtToken(string userName, IdentityUser user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signInCreditials = new SigningCredentials(signInEncodingKey.GetKey(), 
                signInEncodingKey.SignInAlgorithm);

            var encryptedCreditials = new EncryptingCredentials(jwtEncryptionEncodingKey.GetKey(),
                jwtEncryptionEncodingKey.SigningAlgorithm,
                jwtEncryptionEncodingKey.EncryptingAlgorithm);

            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(tokenLifeTimeOptions.Value.AccessTokenLifeTime));
            var now = DateTime.UtcNow;

            var token = new JwtSecurityTokenHandler().CreateJwtSecurityToken(
                authOptions.Value.Issuer,
                authOptions.Value.Audience,
                notBefore: now,
                subject: new ClaimsIdentity(claims),
                expires: expires,
                issuedAt: DateTime.Now,
                signingCredentials: signInCreditials,
                encryptingCredentials: encryptedCreditials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenereteEmailToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, CryptoRandomizer.GetRandomString(16))
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(TokenParameters.EmailTokenLifeTime));

            return crypt.Crypt(CypherConstants.EmailCipherName, token.Payload.SerializeToJson());
        }

        public string DecryptEmailToken(string token)
		{
            var decryptToken = crypt.DecryptString(CypherConstants.EmailCipherName, token);
            return decryptToken;
		}
    }
    internal class TokenParameters
	{
        public const int EmailTokenLifeTime = 14;
        public static readonly TimeSpan Delta = new TimeSpan(0, 0, 30);
	}
}
