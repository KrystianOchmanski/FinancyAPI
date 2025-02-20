﻿using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for user authentication and authorization.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerDTO">User registration data.</param>
        /// <returns>Returns registration result or a list of errors.</returns>
        /// <response code="200">Registration successful.</response>
        /// <response code="400">Invalid input data.</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var result = await _authService.RegisterAsync(registerDTO);
            if(!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }
            return Ok(new { Result = result });
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        /// <param name="loginDTO">User login data.</param>
        /// <returns>Returns access and refresh tokens.</returns>
        /// <response code="200">Login successful.</response>
        /// <response code="401">Invalid email or password.</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authService.LoginAsync(loginDTO);
            if (result == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new
            {
                AccessToken = result.Value.Token,
                RefreshToken = result.Value.RefreshToken
            });
        }

        /// <summary>
        /// Refreshes an access token.
        /// </summary>
        /// <param name="refreshToken">Refresh token.</param>
        /// <returns>Returns a new access token.</returns>
        /// <response code="200">Token refreshed successfully.</response>
        /// <response code="401">Invalid refresh token.</response>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenDTO refreshToken)
        {
            var newToken = _authService.RefreshToken(refreshToken.RefreshToken);
            if (newToken == null)
            {
                return Unauthorized("Invalid refresh token");
            }
            return Ok(new { AccessToken = newToken });
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        /// <returns>Logout confirmation message.</returns>
        /// <response code="200">Logout successful.</response>
        /// <response code="400">Logout failed.</response>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            bool result = await _authService.LogoutAsync(User);

            if (!result)
            {
                return BadRequest("Logout failed.");
            }

            return Ok("User logged out successfully.");
        }
    }
}
