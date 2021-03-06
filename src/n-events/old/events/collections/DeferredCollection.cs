using System.Linq;

namespace N.Package.Events.Legacy
{
    public class DeferredCollection : AbstractListenerCollection
    {
        /// Factory to use to create groups
        protected override IEventListener EventListenerFor(System.Type T)
        {
            var rtn = new DeferredEventListener();
            rtn.AddSupportedType(T);
            return rtn;
        }

        /// Pass an event to all event listener objects
        protected override void Trigger(IEventListener listener, IEvent data)
        {
            foreach (var handler in (listener as DeferredEventListener).Handlers)
            {
                PreInvokeHandler(listener, handler.handler, data);
                handler.localHandler(data);
            }
        }

        /// Bind a delegate that will be invoked when an event occurs
        /// @param callback A callback to invoke.
        public void AddEventHandler<T>(EventHandler<T> callback, float deferInterval) where T : class, IEvent
        {
            var listener = ListenerFor(typeof(T));
            (listener as DeferredEventListener).AddEventHandler(callback, deferInterval);
        }

        /// Step all child elements
        public void Step(float dt)
        {
            foreach (var target in listeners.Values)
            {
                (target as DeferredEventListener).Step(dt);
            }
        }
    }
}
