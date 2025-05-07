using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkinTelIigent.Contracts.DTOs.Notification;
using SkinTelIigent.Contracts.DTOs;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigentContracts.CustomResponses;
using System.Security.Claims;
using SkinTelIigent.Infrastructure.UnitOfWork;
using SkinTelIigent.Core.Entities;

namespace SkinTelligent.Api.Controllers
{
    [Authorize]
    public class NotificationsController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public NotificationsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("/api/notifications")]
        public async Task<ActionResult<PaginationDTO<NotificationDTO>>> GetNotifications([FromQuery] PaginationSpecParams paginationParams)
        {
            var userId = GetAuthenticatedUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new BaseApiResponse(401, "Unauthorized."));

        
            var spec = new NotificationSpecification(userId, paginationParams);
            var notifications = await _unitOfWork.Repository<Notification>().GetAllWithSpecAsync(spec); 
            var notificationsMapper = _mapper.Map<IReadOnlyList<Notification>,IReadOnlyList<NotificationDTO>>(notifications);
            var totalCount = await _unitOfWork.Repository<Notification>()
                .CountWithSpec(new NotificationSpecification(userId));

            var result = new PaginationDTO<NotificationDTO>
            {
                PageIndex = paginationParams.PageIndex,
                PageSize = paginationParams.PageSize,
                Count = totalCount,
                data = notificationsMapper
            };

            return Ok(result);
        
        }
        
        
        [Authorize]
        [HttpGet("/api/notifications/unread-count")]
        public async Task<ActionResult<BaseApiResponse>> GetUnreadCount()
        {
            var userId = GetAuthenticatedUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var unreadCount = await _unitOfWork.Repository<Notification>()
                .CountWithSpec(new NotificationSpecification(userId, onlyUnread: true));

            return Ok( new { UnreadCount = unreadCount });
        }

        [Authorize]
        [HttpPut("/api/notifications/{id}/read")]
        public async Task<ActionResult<BaseApiResponse>> MarkNotificationAsRead(int id)
        {
            var userId = GetAuthenticatedUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new BaseApiResponse(401, "Unauthorized."));

            var notification = await _unitOfWork.Repository<Notification>().GetByIdAsync(id);

            if (notification == null || notification.UserId != userId)
                return NotFound(new BaseApiResponse(404, "Notification not found."));

            if (notification.IsRead)
                return Ok(new BaseApiResponse(200, "Notification already marked as read."));

                await _unitOfWork.BeginTransactionAsync();
                notification.IsRead = true;
                await _unitOfWork.SaveChangeAsync();
                var success = await _unitOfWork.CommitAsync();
                if (!success)
                    return StatusCode(500, new BaseApiResponse(500, "Failed to mark notification as read."));
                return Ok(new BaseApiResponse(200, "Notification marked as read."));
        }

        protected string? GetAuthenticatedUserId()
        {
            return User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
