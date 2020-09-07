using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TPMApi.TokenProvider
{
    public class TokenProviderMiddelware
    {
        private readonly RequestDelegate _request;
        private readonly TokenOptions _options;

        public TokenProviderMiddelware(
            RequestDelegate request,
            IOptions<TokenOptions> options)
        {
            _request = request;
            _options = options.Value;
        }

        //CHECK: add usermanager as Per-request middleware dependencies. 
        //Otherwise you get error: Cannot resolve scoped service .... from root provider
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.1
        public async Task InvokeAsync(HttpContext context, UserManager<IdentityUser> userManager)
        {
            // if the request path does not match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                await _request(context);
            }

            //request must be a POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST") ||
                !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Bad Request");
            }

            //-----------------------------------------------------//

            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            //get userData by name
            var user = await userManager.FindByNameAsync(username);

            //is this a legit user/password else return 400;
            if (await userManager.FindByNameAsync(username) == null ||
                await userManager.CheckPasswordAsync(user, password) == false)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Access Denied");
            }

            //-----------------------------------------------------//

            await GenerateToken(context, user);
        }

        public async Task GenerateToken(HttpContext context, IdentityUser user)
        {
            //set header content-type
            context.Response.ContentType = "application/json";

            //create the neccessary claims
            var claims = new Claim[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.Hour.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddHours(1).ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64),
                };

            //create the jwt
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audiance,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);

            //write the jwt to a string
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                issuer = _options.Issuer,
            };

            //write response.
            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                response,
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented
                }));
        }
    }
}
