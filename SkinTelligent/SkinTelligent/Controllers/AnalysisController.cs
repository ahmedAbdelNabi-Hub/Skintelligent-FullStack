using Microsoft.AspNetCore.Mvc;
using SkinTelIigent.Contracts.DTOs.Analysis;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Models;
using SkinTelIigent.Infrastructure.Repositories;
using SkinTelIigent.Services;
using SkinTelIigentContracts.CustomResponses;

namespace SkinTelligent.Api.Controllers
{
    public class AnalysisController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalysisController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/api/analytics/admin-dashboard")]
        public async Task<ActionResult<AdminDashboardFullDTO>> GetDashboardOverview()
        {
            var AnalysisRepo = _unitOfWork.AnalysisRepository();
            var overviewTask = await AnalysisRepo.GetAdminDashboardOverviewAsync();
            var doctorGrowthTask = await AnalysisRepo.GetDoctorGrowthOverTimeAsync();
            var patientGrowthTask = await AnalysisRepo.GetPatientGrowthOverTimeAsync();
            var appointmentVolumeTask = await AnalysisRepo.GetAppointmentVolumeOverTimeAsync();


            return Ok(new AdminDashboardFullDTO
            {
                Overview = overviewTask,
                DoctorGrowth = doctorGrowthTask,
                PatientGrowth = patientGrowthTask,
                AppointmentVolume = appointmentVolumeTask
            });
        }

        [HttpGet("/api/analytics/appointments/completed")]
        public async Task<ActionResult<List<AppointmentVolumeData>>> GetCompletedAppointments([FromQuery] int? year = null)
        {
            var data = await _unitOfWork.AnalysisRepository().GetCompletedAppointmentsByMonthAsync(year);
            return Ok(data);
        }

        [HttpGet("/api/analytics/dashboard/doctor")]
        public async Task<ActionResult<DashboardCounters>> GetDoctorDashboard()
        {
            var doctorId = GetAuthenticatedDoctorId();
            if (doctorId == null)
                return Unauthorized(new BaseApiResponse(401, "Unauthorized."));

            var result = await _unitOfWork.AnalysisRepository().GetDoctorDashboardCountersAsync(doctorId.Value);
           
            if (result == null)
                return NotFound("No data found for this doctor.");

            return Ok(result);
        }


        private int? GetAuthenticatedDoctorId()
        {
            var doctorIdClaim = User.FindFirst("doctorId");

            if (doctorIdClaim == null || !int.TryParse(doctorIdClaim.Value, out int doctorId))
                return null;

            return doctorId;
        }

    }
}
