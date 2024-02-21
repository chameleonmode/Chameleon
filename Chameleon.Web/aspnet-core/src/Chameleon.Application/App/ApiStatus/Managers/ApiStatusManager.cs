using System;

namespace Chameleon.App
{
    public class ApiStatusManager : IApiStatusManager
    {
        private DateTime _dateTime;

        private bool _status;
        public bool LoginIsFailed
        {
            get => _status;
            set
            {
                _status = value;
                _dateTime = DateTime.UtcNow;
            }
        }

        public bool IsOld => _dateTime < DateTime.UtcNow.AddMinutes(-30);
    }
}
