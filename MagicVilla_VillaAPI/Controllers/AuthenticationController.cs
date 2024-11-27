﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController(IUserRepository userRepository , ApiResponse apiResponse) : ControllerBase
	{
		private readonly IUserRepository _userRepository = userRepository;

		private readonly ApiResponse _apiResponse = apiResponse;

        [HttpPost("Register")]
		public async Task<ActionResult<ApiResponse>> Register(RegisterRequestDto dto)
		{
			if(await _userRepository.IsUnique(dto.UserName))
			{
				LocalUser? user = await _userRepository.Register(dto);
				if(user is null)
				{
					_apiResponse.StatusCode = HttpStatusCode.BadRequest;
					_apiResponse.Errors.Add("Something wrong");
					_apiResponse.IsSuccess = false;
					return BadRequest(_apiResponse);
				}
				_apiResponse.StatusCode = HttpStatusCode.OK;
				_apiResponse.IsSuccess = true;
				_apiResponse.Result = user;
				return Ok(_apiResponse);
			}
			_apiResponse.StatusCode = HttpStatusCode.BadRequest;
			_apiResponse.IsSuccess = false;
			_apiResponse.Errors.Add("User Name Should be Unique");
			return BadRequest(_apiResponse);
		}

		[HttpPost("LogIn")]
		public async Task<ActionResult<ApiResponse>>LogIn(LogInRequestDto dto)
		{
			var response = await _userRepository.LogIn(dto);
			if(response is null)
			{
				_apiResponse.StatusCode = HttpStatusCode.BadRequest;
				_apiResponse.IsSuccess =false;
				_apiResponse.Errors.Add("User Name or password is incorrect");
				return BadRequest(_apiResponse);
			}
			_apiResponse.StatusCode = HttpStatusCode.OK;
			_apiResponse.IsSuccess = true;
			_apiResponse.Result = response;
			return Ok(_apiResponse);
		}
	}
}