using Chameleon.Interfaces.Ioc;
using System;
using System.Collections.Generic;

namespace Chameleon.Interfaces.TimeZone
{
    public interface ITimeZoneService
        : ISingletonDependency
    {
        TimeZoneInfo GetTimeZoneAuto();
        IEnumerable<TimeZoneInfo> GetSystemTimeZones();
        void SetTimeZone(TimeZoneInfo timeZoneInfo);
    }
}
