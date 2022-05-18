using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Proiect.Infrastructure.Data.Auth;
using Proiect.Web.ApiModels;

namespace Proiect.Web.Api
{
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        [HttpGet]
        [Authorize]
        [Route("getUser")]
        public async Task<IActionResult> GetUserDataAsync()
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            var userRoles = await _userManager.GetRolesAsync(user);

            return new JsonResult(new UserDTO(user.Id, user.Email, user.FullName, user.UserName, userRoles));
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password)) return Unauthorized("Parola incorecta!");
            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(userClaims);
            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await _roleManager.FindByNameAsync(userRole);
                if (role == null) continue;

                var roleClaims = await _roleManager.GetClaimsAsync(role);
                authClaims.AddRange(roleClaims);
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YVBy0OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SBM="));

            var validity = 1;
            if (model.RememberMe) validity = 7;
            var token = new JwtSecurityToken(
                "http://localhost:57678/",
                "http://localhost:57678/",
                expires: DateTime.Now.AddDays(validity),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPatch("changePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userManager.FindByNameAsync(userName);

            var changePassword = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (changePassword.Succeeded)
                return Ok();

            return BadRequest(changePassword.Errors.FirstOrDefault()?.Code);
        }
    }
}