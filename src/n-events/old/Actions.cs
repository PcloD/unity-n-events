using System.Collections.Generic;

namespace N.Package.Events.Legacy
{
    /// Actions is a high level api for running tasks
    public class Actions
    {
        /// The timer for this set of actions
        private Timer timer;

        /// The events queue to use; if omitted defaults to the timer one
        private EventHandler events;

        /// Create an instance
        /// @param timer The Timer instance to use.
        public Actions(Timer timer)
        {
            this.timer = timer;
            this.events = timer.Events;
        }

        /// Create an instance
        /// @param timer The Timer instance to use.
        /// @param events The event handler to use if not the timer one.
        public Actions(Timer timer, EventHandler events)
        {
            this.timer = timer;
            this.events = events;
        }

        /// Execute an action
        public void Execute<T>() where T : IAction, new()
        {
            Execute(new T());
        }

        /// Execute an action
        public void Execute(IAction action)
        {
            action.Actions = this;
            action.Timer = this.timer;
            action.Execute();
        }

        /// Execute an action
        public void Execute<T>(EventHandler<ActionCompleteEvent> then) where T : IAction, new()
        {
            Execute(new T(), then);
        }

        /// Execute an action
        public void Execute(IAction action, EventHandler<ActionCompleteEvent> then)
        {
            action.Actions = this;
            action.Timer = this.timer;
            events.AddEventHandler<ActionCompleteEvent>((ep) =>
            {
                if (ep.Is(action))
                {
                    then(ep);
                }
            }, true);
            action.Execute();
        }

        /// Run when an action is complete
        public void Complete(IAction action, bool success = true)
        {
            var evp = new ActionCompleteEvent() { action = action };
            events.Trigger(evp);
        }

        /// Trigger any pending events by calling this at most
        /// AbstractListenerCollection.MAX_EVENTS_PER_TRIGGER will run
        /// per AbstractListenerCollection attached to this object.
        /// @return true If There are pending events
        public bool Pending()
        { return events.PendingEvents > 0; }
    }
}
