/* 
    This file is part of crANNy. Copyright (C) 2017 Christian Rauch.
    Distributed under terms of the GPL3 license.
*/

namespace CRAI.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public interface IEventBus
    {
        void Publish<TMessage>(TMessage message);
        void PublishAsync<TMessage>(TMessage message);
        void Register(Object subscriber);
        void Unregister(Object subscriber);
    }

    public interface ISubscribe<in TMessage>
    {
        void Handle(TMessage message);
    }

    public interface ISubscribeAsync<in TMessage> : ISubscribe<TMessage>
    {
        bool IsHandleCalledAsync(TMessage message);
    }

    public interface ISubscribePriorityBalancing<in TMessage> : ISubscribe<TMessage>
    {
        int? GetHandleCalledPriority(TMessage message);
    }

    public interface ISubscribeForeground<in TMessage> : ISubscribe<TMessage>
    {
        bool IsHandleCalledForeground(TMessage message);
    }

    public interface ICallHandleForeground
    {
        void InvokeForegroundAsync(Action action);
        void InvokeForegroundSync(Action action);
    }

    [Serializable] public class EventBus : IEventBus
    {
        private readonly Object _LockAction = new Object();
        private readonly List<WeakReference> _Subscribers = new List<WeakReference>();
        private readonly ICallHandleForeground _CallHandleForeground;

        public EventBus(ICallHandleForeground callHandleForeground)
        {
            _CallHandleForeground = callHandleForeground;

            var timer = new Timer(delegate 
            { 
                RemoveReferencesNotAlive();
            });

            var timeSpanPeriodicCleanup = TimeSpan.FromMinutes(1);
            timer.Change(timeSpanPeriodicCleanup, timeSpanPeriodicCleanup);
        }

        public IEnumerable<WeakReference> Subscribers
        {
            get
            {
                return _Subscribers;
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            var subscribersAll = ObtainSubscribers<TMessage>().ToList();

            var subscribersAsync
                = subscribersAll
                .OfType<ISubscribeAsync<TMessage>>()
                .Where(s => s.IsHandleCalledAsync(message) 
                    && ObtainSubcriberPriority(s, message) != null)
                .ToList();

            foreach (var subscriberAsync in subscribersAsync)
            {
                var subscribeAsyncx = subscriberAsync;
                new Thread(delegate() { CallHandle(subscribeAsyncx, message, true); }).Start();
            }

            var subscribersSync
                 = subscribersAll
                 .Except(subscribersAsync)
                 .Select(s => new { Target = s, Priority = ObtainSubcriberPriority(s, message) })
                 .Where(s => s.Priority != null)
                 .OrderBy(s => s.Priority)
                 .ToList();

            foreach (var subscriberSync in subscribersSync)
            {
                CallHandle(subscriberSync.Target, message, false);
            }
        }

        private void CallHandle<TMessage>(
            ISubscribe<TMessage> subscriber, TMessage message, bool isAsync)
        {
            var subscriberForeground = subscriber as ISubscribeForeground<TMessage>;

            var isForeground 
                = subscriberForeground != null 
                && subscriberForeground.IsHandleCalledForeground(message);

            if(isForeground)
            {
                var invoke = new Action(delegate { subscriberForeground.Handle(message); });

                if(isAsync)
                {
                    _CallHandleForeground.InvokeForegroundAsync(invoke);
                }
                else
                {
                    _CallHandleForeground.InvokeForegroundSync(invoke);
                }
            }
            else
            {
                subscriber.Handle(message);
            }
        }

        public void PublishAsync<TMessage>(TMessage message)
        {
            new Thread(new ThreadStart(delegate { Publish(message); })).Start();
        }

        public void Register(Object subscriber)
        {
            lock (_LockAction)
            {
                if (subscriber != null
                    && IsNotSubscripted(subscriber)
                    && IsHandlerImplemented(subscriber))
                {
                    _Subscribers.Add(new WeakReference(subscriber));
                }
                else
                {
                    throw new ArgumentException(
                        "ISubscribe<TMessage> not implemented or Register called multiple times: "
                        + (subscriber == null ? "NULL" : subscriber.GetType().FullName));
                }
            }
        }

        public void Unregister(Object subscriber)
        {
            lock (_LockAction)
            {
                var weakReferenceToRemove
                    = _Subscribers
                    .FirstOrDefault(s => s.Target == subscriber);

                _Subscribers.Remove(weakReferenceToRemove);
            }
        }

        private IEnumerable<ISubscribe<TMessage>> ObtainSubscribers<TMessage>()
        {
            lock (_LockAction)
            {
                var subscribersFiltered
                    = _Subscribers
                    .Select(s => s.Target as ISubscribe<TMessage>)
                    .Where(s => s != null)
                    .ToList();

                return subscribersFiltered;
            }
        }

        private int? ObtainSubcriberPriority<TMessage>(ISubscribe<TMessage> subscriber, TMessage message)
        {
            var subscriberPriority = subscriber as ISubscribePriorityBalancing<TMessage>;

            var priority = subscriberPriority != null 
                ? subscriberPriority.GetHandleCalledPriority(message) 
                : 0;

            return priority;
        }

        private bool IsHandlerImplemented(Object subscriber)
        {
            var interfaces
                = subscriber.GetType()
                .FindInterfaces(
                    delegate(Type type, Object criteria)
                    {
                        return
                            type.IsGenericType
                            && type.GetGenericTypeDefinition() == typeof(ISubscribe<>);
                    },
                    null);

            return interfaces.Length > 0;
        }

        private bool IsNotSubscripted(Object subscriber)
        {
            lock (_LockAction)
            {
                return _Subscribers.All(o => o.Target != subscriber);
            }
        }

        private void RemoveReferencesNotAlive()
        {
            lock (_LockAction)
            {
                var weakReferences
                    = _Subscribers
                    .Where(s => !s.IsAlive)
                    .ToList();

                weakReferences
                    .ForEach(reference => _Subscribers.Remove(reference));
            }
        }
    }
}
