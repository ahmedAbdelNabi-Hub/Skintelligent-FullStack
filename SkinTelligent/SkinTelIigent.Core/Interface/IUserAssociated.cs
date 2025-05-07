using SkinTelIigent.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkinTelIigent.Core.Interface
{
    public interface IUserAssociated
    {
        ApplicationUser User { get; }

    }
}
