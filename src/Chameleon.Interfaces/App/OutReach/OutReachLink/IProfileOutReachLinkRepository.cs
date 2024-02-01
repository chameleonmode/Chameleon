using Chameleon.Interfaces.Repository;
using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.OutReach
{
    public interface IProfileOutReachLinkRepository
        : IRepository<IProfileOutReachLink, int, UserProfileGetAllRequestDto>
    {
        List<IProfileOutReachLink> GetAll(DateTime reminderDate);
    }
}
