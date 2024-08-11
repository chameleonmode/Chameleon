namespace Chameleon.Interfaces;

public interface IHaveInitialize
{
    TaskCompletionSource LoadedTCS { get; }
    Task InvokeInitializeAsyncCommand(object? param);
}
