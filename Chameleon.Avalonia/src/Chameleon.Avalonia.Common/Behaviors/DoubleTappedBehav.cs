using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using System.Windows.Input;
using System;

namespace Chameleon.Avalonia.Common.Behaviors;

/// <summary>
/// Class to create an <see cref="IObserver{T}"/> instance from delegate-based implementations of the On* methods.
/// </summary>
/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
public class AnonymousObserver<T> : IObserver<T>
{
    private static readonly Action<Exception> ThrowsOnError = ex => throw ex;
    private static readonly Action NoOpCompleted = () => { };
    private readonly Action<T> _onNext;
    private readonly Action<Exception> _onError;
    private readonly Action _onCompleted;

    public AnonymousObserver(TaskCompletionSource<T> tcs)
    {
        if (tcs is null)
        {
            throw new ArgumentNullException(nameof(tcs));
        }

        _onNext = tcs.SetResult;
        _onError = tcs.SetException;
        _onCompleted = NoOpCompleted;
    }

    public AnonymousObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted)
    {
        _onNext = onNext ?? throw new ArgumentNullException(nameof(onNext));
        _onError = onError ?? throw new ArgumentNullException(nameof(onError));
        _onCompleted = onCompleted ?? throw new ArgumentNullException(nameof(onCompleted));
    }

    public AnonymousObserver(Action<T> onNext)
        : this(onNext, ThrowsOnError, NoOpCompleted)
    {
    }

    public AnonymousObserver(Action<T> onNext, Action<Exception> onError)
        : this(onNext, onError, NoOpCompleted)
    {
    }

    public AnonymousObserver(Action<T> onNext, Action onCompleted)
        : this(onNext, ThrowsOnError, onCompleted)
    {
    }

    public void OnCompleted()
    {
        _onCompleted.Invoke();
    }

    public void OnError(Exception error)
    {
        _onError.Invoke(error);
    }

    public void OnNext(T value)
    {
        _onNext.Invoke(value);
    }
}

/// <summary>
/// Container class for attached properties. Must inherit from <see cref="AvaloniaObject"/>.
/// </summary>
public class DoubleTappedBehav : AvaloniaObject
{
    static DoubleTappedBehav()
    {
        //CommandProperty.Changed.Subscribe(x => HandleCommandChanged(x.Sender, x.NewValue.GetValueOrDefault<ICommand>()));
        CommandProperty.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(OnValueChanged));
    }

    private static void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
       
    }



    /// <summary>
    /// Identifies the <seealso cref="CommandProperty"/> avalonia attached property.
    /// </summary>
    /// <value>Provide an <see cref="ICommand"/> derived object or binding.</value>
    public static readonly AttachedProperty<ICommand> CommandProperty = AvaloniaProperty.RegisterAttached<DoubleTappedBehav, Interactive, ICommand>(
        "Command", default(ICommand), false, BindingMode.OneTime);

    /// <summary>
    /// Identifies the <seealso cref="CommandParameterProperty"/> avalonia attached property.
    /// Use this as the parameter for the <see cref="CommandProperty"/>.
    /// </summary>
    /// <value>Any value of type <see cref="object"/>.</value>
    public static readonly AttachedProperty<object> CommandParameterProperty = AvaloniaProperty.RegisterAttached<DoubleTappedBehav, Interactive, object>(
        "CommandParameter", default(object), false, BindingMode.OneWay, null);


    /// <summary>
    /// <see cref="CommandProperty"/> changed event handler.
    /// </summary>
    private static void HandleCommandChanged(AvaloniaObject element, ICommand commandValue)
    {
        if (element is Interactive interactElem)
        {
            if (commandValue != null)
            {
                // Add non-null value
                interactElem.AddHandler(InputElement.DoubleTappedEvent, Handler);
            }
            else
            {
                // remove prev value
                interactElem.RemoveHandler(InputElement.DoubleTappedEvent, Handler);
            }
        }

        // local handler fcn
        static void Handler(object s, RoutedEventArgs e)
        {
            if (s is Interactive interactElem)
            {
                // This is how we get the parameter off of the gui element.
                object commandParameter = interactElem.GetValue(CommandParameterProperty);
                ICommand commandValue = interactElem.GetValue(CommandProperty);
                if (commandValue?.CanExecute(commandParameter) == true)
                {
                    commandValue.Execute(commandParameter);
                }
            }
        }
    }


    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static void SetCommand(AvaloniaObject element, ICommand commandValue)
    {
        element.SetValue(CommandProperty, commandValue);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandProperty"/>.
    /// </summary>
    public static ICommand GetCommand(AvaloniaObject element)
    {
        return element.GetValue(CommandProperty);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandParameterProperty"/>.
    /// </summary>
    public static void SetCommandParameter(AvaloniaObject element, object parameter)
    {
        element.SetValue(CommandParameterProperty, parameter);
    }

    /// <summary>
    /// Accessor for Attached property <see cref="CommandParameterProperty"/>.
    /// </summary>
    public static object GetCommandParameter(AvaloniaObject element)
    {
        return element.GetValue(CommandParameterProperty);
    }
}
