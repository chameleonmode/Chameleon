using Chameleon.Interfaces.Ioc;
using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.OutReach
{
    public interface IProfileOutReachLinkService 
        : Chameleon.lib.Common.Interfaces.Systemics.ISingletonDependency
    {
        IProfileOutReachLinks GetAll(int profileId);
        void Update(IProfileOutReachLink outReachLink);
        void Delete(IProfileOutReachLink outReachLink);
        IProfileOutReachLink Save(IProfileOutReachLink outReachLink);
        IProfileOutReachLinks GetAllByReminderDate(DateTime reminderDate);
    }
}
