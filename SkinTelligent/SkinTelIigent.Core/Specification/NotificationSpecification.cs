using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Specification
{
    public class NotificationSpecification : BaseSpecifications<Notification>
    {
        public NotificationSpecification()
        {
            
        }
        public NotificationSpecification(string userId)
        {
            AddCriteria(n => n.UserId == userId && !n.IsRead);
        }
        public NotificationSpecification(string userId, bool onlyUnread)
            : base(n => n.UserId == userId && (!onlyUnread || !n.IsRead))
        {
            AddOrderByDescending(n => n.CreatedAt);
        }
        public NotificationSpecification(string userId, PaginationSpecParams paginationParams)
            : base(n => n.UserId == userId)
        {
            ApplyPagination(paginationParams.PageSize * (paginationParams.PageIndex - 1), paginationParams.PageSize);
            AddOrderByDescending(n => n.CreatedAt);
        }
    }
}
