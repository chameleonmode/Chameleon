namespace Chameleon;
//
// Summary:
//     The result of the dialog.
public enum ButtonResult
{
    //
    // Summary:
    //     Abort.
    Abort = 3,
    //
    // Summary:
    //     Cancel.
    Cancel = 2,
    //
    // Summary:
    //     Ignore.
    Ignore = 5,
    //
    // Summary:
    //     No.
    No = 7,
    //
    // Summary:
    //     No result returned.
    None = 0,
    //
    // Summary:
    //     OK.
    OK = 1,
    //
    // Summary:
    //     Retry.
    Retry = 4,
    //
    // Summary:
    //     Yes.
    Yes = 6
}

public class PubSubEvent
{

}
public class PubSubEvent<T> : EventBase
{
    /// <summary>
    /// Publishes the <see cref="PubSubEvent{TPayload}"/>.
    /// </summary>
    /// <param name="payload">Message to pass to the subscribers.</param>
    public virtual void Publish(T payload)
    {
    }
}

public class BindableBase
{ }

    //
    // Summary:
    //     Defines an interface to get instances of an event type.
    public interface IEventAggregator
    {
        //
        // Summary:
        //     Gets an instance of an event type.
        //
        // Type parameters:
        //   TEventType:
        //     The type of event to get.
        //
        // Returns:
        //     An instance of an event object of type TEventType.
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
    }

    public class EventBase
    {
    }


