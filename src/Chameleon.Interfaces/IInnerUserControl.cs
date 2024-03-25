namespace Chameleon.Interfaces;

public interface IHaveInitialize
{
    Task InvokeInitializeAsyncCommand(object? param);
}
