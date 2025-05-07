using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Contracts.Interface
{
    public interface INotificationSender
    {
        Task SendAsync(string? userId, string message, int count);
        Task<int> CreateAndSendAsync(string userId, string message, string title , string type);
    }

}
