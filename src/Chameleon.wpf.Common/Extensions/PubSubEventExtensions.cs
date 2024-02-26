using Prism.Events;
using System;

namespace Chameleon.Common.Extensions
{
    public static class PubSubEventExtensions
    {
        public static void Publish<TPayload>(this PubSubEvent<TPayload> self)
            where TPayload : class, new()
        {
            self.Publish(new TPayload());
        }

        public static SubscriptionToken SubscribeOnUIThread<TPayload>(this PubSubEvent<TPayload> self, 
            Action<TPayload> action, bool keepSubscriberReferenceAlive = true)
        {
            return self.Subscribe(action, ThreadOption.UIThread, keepSubscriberReferenceAlive);
        }

        public static SubscriptionToken SubscribeOnBackgroundThread<TPayload>(this PubSubEvent<TPayload> self,
            Action<TPayload> action, bool keepSubscriberReferenceAlive = true)
        {
            return self.Subscribe(action, ThreadOption.BackgroundThread, keepSubscriberReferenceAlive);
        }

        public static IDisposable SubscribeOnce<TPayload>(this PubSubEvent<TPayload> self,
            Action<TPayload> action, bool keepSubscriberReferenceAlive = true)
        {
            var subscription = new PubSubSubscription();
            var subscriptionToken = self.Subscribe((payload) => {
                if (subscription.HasToken)
                {
                    subscription.Dispose();
                    action(payload);
                }
            }, keepSubscriberReferenceAlive);
            subscription.Token = subscriptionToken;
            return subscription;
        }

        public static IDisposable SubscribeOnce<TPayload>(this PubSubEvent<TPayload> self,
            Action action, bool keepSubscriberReferenceAlive = true)
        {
            return self.SubscribeOnce(_ => action(), 
                keepSubscriberReferenceAlive);
        }

        private class PubSubSubscription : IDisposable
        {
            public SubscriptionToken Token { get; set; }
            public bool HasToken => Token != null;

            public void Dispose()
            {
                Token?.Dispose();
                Token = null;
            }
        }
    }
}
