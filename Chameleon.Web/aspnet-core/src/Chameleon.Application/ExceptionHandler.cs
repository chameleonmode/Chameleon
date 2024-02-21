using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using System;

namespace Chameleon
{
    public class ExceptionHandler 
        : IEventHandler<AbpHandledExceptionData>
        , ITransientDependency
    {
        protected ILogger Logger { get; set; }
        public ExceptionHandler(ILogger logger)
        {
            Logger = logger;
        }

        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            var exception = eventData.Exception;

            Logger.Error(
                $"ExceptionHandler: {GetExceptionMessage(exception)}", 
                exception);
        }

        private string GetExceptionMessage(Exception ex)
        {
            var innerExceptionnner = ex.InnerException;
            if (innerExceptionnner == null)
            {
                return ex.Message;
            }

            if (string.IsNullOrEmpty(innerExceptionnner.Message))
            {
                return ex.Message;
            }
            return GetExceptionMessage(innerExceptionnner);
        }
    }
}
