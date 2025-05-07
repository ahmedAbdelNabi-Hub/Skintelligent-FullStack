using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SkinTelIigent.Contracts.DTOs.Authentication;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigentContracts.CustomResponses;
using SkinTelligent.Api.Helper.MappingProfile;
using SkinTelligent.Api.Helper.Upload;
using System.Security.Claims;

namespace SkinTelligent.Api.Controllers
{
    public class AuthenticationController : BaseController
    {   
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        IEmailService _emailService;    
        public AuthenticationController(UserManager<ApplicationUser> userManager, IAuthService authService,IMapper mapper,IEmailService emailService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("/api/auth/login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseApiResponse>> login([FromBody] LoginDTO loginDTO){
                          
          var responseFromAuth = await _authService.loginAsync(loginDTO); 
            
          return HandleStatusCode(responseFromAuth);    

        }


        [HttpPost("/api/auth/patient/register")]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterPatient([FromForm] RegisterPatientDTO dto)
        {
            var imageName = "default.jpg";

            if (dto?.ProfilePicture != null)
                imageName = DocumentSettings.UploadFile(dto.ProfilePicture, "PatientPictures");
            var response = await _authService.registerPatientAsync(dto!.ToApplicationUser(imageName!), dto!.Password);
            return response.statusCode == StatusCodes.Status200OK ? Ok(response) : BadRequest(response);
        }

        [HttpPost("/api/auth/clinic/register")]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterClinic([FromForm] RegisterClinicDTO dto)
        {
            var imageName = "default.jpg";
            if (dto.Image != null)
                imageName = DocumentSettings.UploadFile(dto.Image, "clinicProfilePictures");
            var response = await _authService.registerClinicAsync(dto.ToApplicationUser(imageName), dto.Password);
            return response.statusCode == StatusCodes.Status200OK ? Ok(response) : BadRequest(response);
        }


        [HttpPost]
        [Route("/api/auth/register")]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromForm] RegisterDTO registerDTO, [FromQuery] string registerType)
        {
            var imageName = "default.jpg";
            if (registerType == "clinic" && registerDTO.Clinic?.Image != null)
                imageName = DocumentSettings.UploadFile(registerDTO.Clinic.Image, "clinicPictures");

            if (registerType == "patient" && registerDTO.Patient?.ProfilePicture != null)
                imageName = DocumentSettings.UploadFile(registerDTO.Patient.ProfilePicture, "PatientPictures");


            var registrationHandlers = new Dictionary<string, Func<Task<BaseApiResponse>>>
            {
                { "patient", () => _authService.registerPatientAsync(registerDTO.Patient!.ToApplicationUser(imageName!), registerDTO.Patient!.Password) },
                { "clinic", () => _authService.registerClinicAsync(registerDTO.Clinic!.ToApplicationUser(imageName!), registerDTO.Clinic!.Password) }
            };

            if (!registrationHandlers.TryGetValue(registerType, out var handler))
                return BadRequest(new BaseApiResponse { statusCode = StatusCodes.Status400BadRequest, message = "Invalid registration type" });

            var response = await handler();
            return response.statusCode == StatusCodes.Status200OK ? Ok(response) : BadRequest(response);
        }


        [HttpPost]
        [Route("/api/auth/forgot-password")]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseApiResponse>> ForgotPasswordAsync([FromBody] ForgetPasswordDTO forgotPassword)
        {
            var response = await _authService.ForgotPasswordAsync(forgotPassword);
           
            return HandleStatusCode(response);

        }
        [HttpPost]
        [Route("/api/auth/reset-password")]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseApiResponse>> RestPassword([FromBody] RestPasswordDTO restPasswordDto)
        {

            var response = await _authService.RestPasswordAsync(restPasswordDto);
            return HandleStatusCode(response);

        }

        [HttpPost]
        [Route("/api/auth/confirm-email")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseApiResponse>> ConfirmEmail(ConfirmEmailDTO confirmEmailDto)
        {
            var response = await _authService.ConfirmEmailAsync(confirmEmailDto);
            return HandleStatusCode(response);
        }

        [HttpPost("/api/auth/send-email")]
        public async Task<ActionResult<BaseApiResponse>> SendEmail([FromForm] EmailRequestDTO request)
        {
            var IsSend = await _emailService.SendEmailAsync(request.EmailTo, request.Subject, request.Body,request.Attachments);
            if (IsSend) return Ok(new BaseApiResponse(200, "Email sent successfully."));

            return StatusCode(StatusCodes.Status500InternalServerError, new BaseApiResponse(500, "Failed to send email."));
        }

        [HttpDelete]
        [Route("/api/user/{id}")]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseApiResponse>> DeleteAccount(int id, [FromQuery] string TypeUser)
        {
            var response = await (TypeUser.ToLower() switch
            {
                "patient" => _authService.DeleteAccount<Patient>(id),
                "doctor" => _authService.DeleteAccount<Doctor>(id),
                "clinic" => _authService.DeleteAccount<Clinic>(id),
                _ => Task.FromResult(new BaseApiResponse(400, "Invalid user type specified.")) // Default case
            });

            return StatusCode(response.statusCode, response);
        }

        [Authorize]
        [HttpGet]
        [Route("/api/auth/current-user")]
        public IActionResult GetCurrentUser()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return Unauthorized();

            var currentUser = new
            {
                Username = User.Identity?.Name,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList()
            };

            return Ok(currentUser);
        }


        [HttpPut("/api/users/{id}/block")]
        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new BaseApiResponse(StatusCodes.Status404NotFound, "User not found"));

            user.LockoutEnabled = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(new BaseApiResponse(StatusCodes.Status400BadRequest, "Failed to block user"));

            return Ok(new BaseApiResponse(StatusCodes.Status200OK, "User blocked successfully"));
        }



    }
}

