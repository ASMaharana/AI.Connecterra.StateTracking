using System;
using System.Collections.Generic;
using Connecterra.StateTracking.Persistent.EventStore;
using Newtonsoft.Json.Linq;

namespace Connecterra.StateTracking.Persistent.Projections
{
    public interface IProjection
    {
        bool IsSubscribedTo(IEvent @event);

        string GetViewName(DateTime timeStamp);

        void Apply(IEvent @event, IView view);
    }
    public abstract class CosmosProjection<TView> : IProjection
        where TView : new()
    {
        private readonly Dictionary<Type, Action<IEvent, object>> _handlers;

        public CosmosProjection()
        {
            _handlers = new Dictionary<Type, Action<IEvent, object>>();
        }

        public bool IsSubscribedTo(IEvent @event) => _handlers.ContainsKey(@event.GetType());

        public virtual string GetViewName(DateTime timeStamp) => typeof(TView).Name;

        public void Apply(IEvent @event, IView view)
        {
            var payload = view.Payload.ToObject<TView>();

            var eventType = @event.GetType();
            if (_handlers.TryGetValue(eventType, out Action<IEvent, object> handler))
            {
                handler(@event, payload);

                view.Payload = JObject.FromObject(payload);
            }
        }

        protected void RegisterHandler<TEvent>(Action<TEvent, TView> handler)
            where TEvent : IEvent
        {
            _handlers[typeof(TEvent)] = new Action<IEvent, object>((e, v) => handler((TEvent)e, (TView)v));
        }
    }
}