﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PointOfSales.Entities;
using PointOfSales.Interfaces;
using PointOfSales.Services;
using System;
using System.Threading.Tasks;
using WebApisPointOfSales.MiddleWares;
using AutoMapper;
using WebApisPointOfSales.Dto.UserDtos;

namespace WebApisPointOfSales.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersManagerController : ControllerBase
    {
        private readonly ILogger<UsersManagerController> _logger;
        private readonly IUserManagerService _userManagerService;
        private readonly AuthBearerMiddleware _authService;
        private readonly IMapper _mapper;

        public UsersManagerController(IUserManagerService userManagerService, ILogger<UsersManagerController> logger, AuthBearerMiddleware authService, IMapper mapper)
        {
            _userManagerService = userManagerService;
            _logger = logger;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto request)
        {
            try
            {
                _logger.LogInformation("Attempting to register user with email: {Email}", request.Email);

                // Map RegisterUserDto to Users entity
                var userEntity = _mapper.Map<Users>(request);

                var registeredUser = await _userManagerService.RegisterUser(userEntity);
                _logger.LogInformation("User registered successfully with email: {Email}", request.Email);
                return Ok("User registered successfully.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error registering user with email: {Email}", request.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogInUserAuthentication([FromBody] LoginUserDto request)
        {
            try
            {
                _logger.LogInformation("Attempting to log in user with email: {Email}", request.Email);
                var user = await _userManagerService.LogInUserAuthentication(request.Email, request.Password);
                if (user == null)
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", request.Email);
                    return Unauthorized("Invalid credentials.");
                }

                _logger.LogInformation("User logged in successfully with email: {Email}", request.Email);
                var token = _authService.GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error logging in user with email: {Email}", request.Email);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("changeRole")]
        public async Task<IActionResult> ChangeUserRole([FromQuery] string email, [FromQuery] string newRole)
        {
            try
            {
                _logger.LogInformation("Attempting to change role for user with email: {Email}", email);
                if (await _userManagerService.ChangeUserRole(email, newRole))
                {
                    _logger.LogInformation("User role updated successfully for email: {Email}", email);
                    return Ok("User role updated successfully.");
                }
                _logger.LogWarning("User not found with email: {Email}", email);
                return NotFound("User not found.");
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error changing role for user with email: {Email}", email);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve all users.");
                var users = await _userManagerService.GetAllUsers();
                _logger.LogInformation("Successfully retrieved all users.");
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users.");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
    }
}
