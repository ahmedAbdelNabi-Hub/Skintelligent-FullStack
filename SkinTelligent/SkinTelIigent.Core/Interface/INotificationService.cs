using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Interface
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string receiverUserId, string receiverUserType, string title, string message, object data);
    }
}
