using AngularSecurity.Api.EntityClasses;
using AngularSecurity.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AngularSecurity.Api.ManagerClasses
{
    public class SecurityManager
    {
        public SecurityManager(PtcDbContext context, UserAuthBase auth, JwtSettings settings)
        {
            this.dbContext = context;
            this.auth = auth;
            this.settings = settings;
        }

        private PtcDbContext dbContext = null;
        private UserAuthBase auth = null;
        private readonly JwtSettings settings;

        protected List<UserClaim> GetUserClaims(Guid userId)
        {
            List<UserClaim> list = new List<UserClaim>();

            try
            {
                list = dbContext.Claims.Where(u => u.UserId == userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Exception trying to retrieve user claims.", ex);
            }

            return list;
        }

        protected UserAuthBase BuildUserAuthObject(Guid userId, string userName)
        {
            // Set User Properties
            auth.UserId = userId;
            auth.UserName = userName;
            auth.IsAuthenticated = true;

            // Get all claims for this user
            auth.Claims = GetUserClaims(userId);

            // Create JWT Bearer Token
            auth.BearerToken = BuildJwtToken(auth.Claims, userName);
            return auth;
        }


        public UserAuthBase ValidateUser(string userName, string password)
        {
            List<UserBase> list = new List<UserBase>();

            try
            {
                list = dbContext.Users.Where(
                  u => u.UserName.ToLower() == userName.ToLower()
                  && u.Password.ToLower() == password.ToLower()).ToList();

                if (list.Count() > 0)
                {
                    auth = BuildUserAuthObject(list[0].UserId, userName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception while trying to retrieve user.", ex);
            }



            return auth;
        }

        protected string BuildJwtToken(IList<UserClaim> claims, string userName)
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

            // Create standard JWT claims
            List<Claim> jwtClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add custom claims
            foreach (UserClaim claim in claims)
            {
                jwtClaims.Add(new Claim(claim.ClaimType, claim.ClaimValue));
            }

            // Create the JwtSecurityToken object
            var token = new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: jwtClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(settings.MinutesToExpiration),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            // Create a string representation of the Jwt token
            return new JwtSecurityTokenHandler().WriteToken(token); ;
        }
    }
}
