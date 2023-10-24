using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyContactBook.Core.Services.Interface;
using MyContactBook.Data.DTO;
using MyContactBook.Utilities;

namespace MyContactAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("all-users")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetAllUsers([FromQuery]PagingParameter pagingParameter)
        {
            
            var userDtos = await _userService.GetAllUsersAsync(pagingParameter);

            if (!userDtos.Any())
            {
                return NotFound();
            }

            return Ok(userDtos);

           
            
        }

        [HttpPut("update-details/{userId}")]
        public async Task<ActionResult> UpdateUserDetails(string userId, [FromBody] UpdateUserDetailsDto updateUserDetailsDto)
        {
            if (updateUserDetailsDto == null)
            {
                return BadRequest("Invalid update data.");
            }

            var result = await _userService.UpdateUserDetailsAsync(userId, updateUserDetailsDto);

            if (result)
            {
                return Ok("User details updated successfully.");
            }
            else
            {
                return BadRequest("User details update failed.");
            }
        }

        [HttpPatch("update-photo/{userId}")]
        public async Task<ActionResult> UpdateUserPhoto(string userId, [FromBody] UpdatePhotoDto updatePhotoDto)
        {
            try
            {

                var result = await _userService.UpdateUserPhotoAsync(userId, updatePhotoDto);

                if (result)
                {
                    return Ok("User photo updated successfully.");
                }
                else
                {
                    return BadRequest("User photo update failed.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var result = await _userService.DeleteUserAsync(userId);

            if (result)
            {
                return Ok("User deleted successfully.");
            }
            else
            {
                return BadRequest("User deletion failed.");
            }
        }

        [HttpGet("search-users")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> SearchUsers([FromQuery] string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return BadRequest("Invalid search query.");
            }

            var searchResults = await _userService.SearchUsersAsync(searchQuery);

            if (!searchResults.Any())
            {
                return NotFound("No matching users found.");
            }

            return Ok(searchResults);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<AppUserDto>> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var userDto = await _userService.GetUserByIdAsync(userId);

            if (userDto == null)
            {
                return NotFound("User not found.");
            }

            return Ok(userDto);
        }
    }
}
