using System;
using System.Windows;

namespace Chameleon.Common.Extensions
{
    public static class EventHandlerExtensions
    {
        public static void Clear(this DependencyPropertyChangedEventHandler self)
        {
            if (self == null)
            {
                return;
            }

            foreach (DependencyPropertyChangedEventHandler eventHandler in self.GetInvocationList())
            {
                self -= eventHandler;
            }
        }

        public static void Clear(this EventHandler self)
        {
            if (self == null)
            {
                return;
            }

            foreach (EventHandler eventHandler in self.GetInvocationList())
            {
                self -= eventHandler;
            }
        }

        public static void Clear<TEventArgs>(this EventHandler<TEventArgs> self)
        {
            if (self == null)
            {
                return;
            }

            foreach (EventHandler<TEventArgs> eventHandler in self.GetInvocationList())
            {
                self -= eventHandler;
            }
        }

        public static void Clear<TEventArgs>(this EventHandler<TEventArgs> self, Action<EventHandler<TEventArgs>> removed)
        {
            if (self == null)
            {
                return;
            }

            foreach (EventHandler<TEventArgs> eventHandler in self.GetInvocationList())
            {
                self -= eventHandler;
                removed(eventHandler);
            }
        }
    }
}
