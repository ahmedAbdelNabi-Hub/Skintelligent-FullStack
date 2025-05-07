using Microsoft.AspNetCore.SignalR;
using SkinTelIigent.Contracts.Interface;
using SkinTelIigent.Core.Entities;
using SkinTelIigent.Core.Interface;
using SkinTelIigent.Core.Specification;
using SkinTelIigent.Infrastructure.UnitOfWork;
using SkinTelligent.Api.Hubs;

namespace SkinTelligent.Api.RealTime
{
    public class SignalRNotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        public SignalRNotificationSender(IUnitOfWork unitOfWork , IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;

        }

        public async Task SendAsync(string? userId, string message, int count)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
                {
                    message,
                    count
                });
            }
        }
        public async Task<int> CreateAndSendAsync(string userId, string message,string title,string type)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return 0;

            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Title = title,
                Type = type 
            };

            await _unitOfWork.Repository<Notification>().AddAsync(notification);
            await _unitOfWork.SaveChangeAsync();

            int unreadCount = await _unitOfWork.Repository<Notification>()
                .CountWithSpec(new NotificationSpecification(userId));

            await SendAsync(userId, message, unreadCount);

            return unreadCount;
        }
    }

}
