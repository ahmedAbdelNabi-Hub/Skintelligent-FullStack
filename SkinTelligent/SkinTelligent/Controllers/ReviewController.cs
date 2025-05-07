using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkinTelIigent.Contracts.DTOs;
using SkinTelIigent.Contracts.DTOs._Review;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Infrastructure.Data;
using SkinTelIigentContracts.CustomResponses;

namespace SkinTelligent.Api.Controllers
{
    public class ReviewController : BaseController
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly INotificationSender _notificationSender;

        public ReviewController(INotificationSender notificationSender,IUnitOfWork unitOfWork , IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _notificationSender = notificationSender;
        }


        [Authorize(Roles = "patient")]
        [HttpPost("/api/reviews")]
   
        public async Task<ActionResult<BaseApiResponse>> CreateReview([FromBody] CreateReviewDTO dto)
        {
            var patientId = GetAuthenticatedPatientId();
            if (patientId == null)
                return Unauthorized(new BaseApiResponse(401, "Invalid or Unauthorized Patient."));

            // Fetch doctor, clinic, and patient in parallel for performance
            var doctor = await _unitOfWork.Repository<Doctor>().GetByIdAsync(dto.DoctorId);
            var patient = await _unitOfWork.Repository<Patient>().GetByIdAsync(patientId.Value);
            var clinic = await _unitOfWork.Repository<Clinic>().GetByIdAsync(dto.ClinictId);

            if (doctor == null || patient == null || clinic == null)
                return BadRequest(new BaseApiResponse(400, "Doctor, patient or clinic not found."));

            var existingReview = await _unitOfWork.Repository<Review>()
                .GetByIdSpecAsync(new ReviewSpecifications(dto.DoctorId, patientId.Value, dto.ClinictId));

            if (existingReview != null)
                return BadRequest(new BaseApiResponse(400, "Review already exists from this patient."));

            await _unitOfWork.BeginTransactionAsync();

            var review = new Review
            {
                DoctorId = dto.DoctorId,
                ClinicId = dto.ClinictId,
                PatientId = patientId.Value,
                Comment = dto.Comment,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Review>().AddAsync(review);
            await _unitOfWork.SaveChangeAsync();

            var doctorNotificationTask = await _notificationSender.CreateAndSendAsync(
                                      doctor.UserId,
                                      $"You’ve received a new review from patient {patient.FirstName + " " + patient.LastName}.",
                                      "New Patient Review",
                                      "Review");


            var clinicNotificationTask = await _notificationSender.CreateAndSendAsync(
                             clinic.UserId,
                             $"Doctor {doctor.FirstName +" " + doctor.LastName} has been reviewed by patient {patient.FirstName + " " + patient.LastName} at your clinic.",
                             "Doctor Reviewed at Your Clinic",
                             "Review");


            var isSuccess = await _unitOfWork.CommitAsync();
            if (!isSuccess)
                return BadRequest(new BaseApiResponse(400, "Failed to create review."));


            return Ok(new BaseApiResponse(200, "Review created successfully."));
        }



        [HttpGet("/api/reviews/doctor/{id}")]
        public async Task<ActionResult<PaginationDTO<ReviewDTO>>> GetReviewsByDoctor( int id, [FromQuery] PaginationSpecParams paginationParams)
        {
            var spec = new ReviewSpecifications(id, paginationParams, includePatient: true);
            var countSpec = new ReviewSpecifications(id); 
            var apiBaseUrl = _configuration["ApiBaiseUrl"];
            var totalCount = await _unitOfWork.Repository<Review>().CountWithSpec(countSpec);
            var reviews = await _unitOfWork.Repository<Review>()
                .GetProjectedAsync(r => new ReviewDTO
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    PatientName = r.Patient.FirstName + " " + r.Patient.LastName,
                    PatientImage = !string.IsNullOrEmpty(r.Patient.ProfilePicture)
                        ? $"{apiBaseUrl}/PatientImage/{r.Patient.ProfilePicture}"
                        : ""
                }, spec);

            var result = new PaginationDTO<ReviewDTO>
            {
                PageIndex = paginationParams.PageIndex,
                PageSize = paginationParams.PageSize,
                Count = totalCount,
                data = reviews
            };

            return Ok(result);
        }

        private int? GetAuthenticatedPatientId()
        {
            var patientIdClaim = User.FindFirst("patientId");

            if (patientIdClaim == null || !int.TryParse(patientIdClaim.Value, out int patientId))
                return null;

            return patientId;
        }



    }
}
