using Chameleon.Domain.Entities;
using Chameleon.Interfaces.ExceptionOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chameleon.Infrastructure.ExceptionOptions
{
    public class AppLoggerService
        : IAppLoggerService
    {
        private readonly IAppLoggerRepository _repository;

        public AppLoggerService(
            IAppLoggerRepository repository
            )
        {
            _repository = repository;
        }

        private readonly IAppLoggers _loggs = new AppLoggers();
        public IAppLoggers Loggs => _loggs;

        public void Create(IAppLogger appLogger)
        {
            _repository.Insert(appLogger);
            _loggs.Add(appLogger);
        }
    }
}
