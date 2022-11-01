using CQRS.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.Core.Domain
{
    public abstract class AggregateRoot
    {
        protected Guid id;

        private readonly List<BaseEvent> changes = new();

        public Guid Id
        {
            get { return id; }
        }

        public int Version { get; set; } = -1;

        public IEnumerable<BaseEvent> GetUnCommittedChanges()
        {
            return changes;
        }

        public void MarkChangesAsCommitted()
        {
            changes.Clear();
        }

        private void ApplyChange(BaseEvent baseEvent, bool isNew)
        {
            var method = this.GetType().GetMethod("Apply", new Type[] { baseEvent.GetType() });

            if (method == null)
            {
                throw new ArgumentNullException(nameof(method), $"The Apply method was not found in the aggregate for {baseEvent.GetType().Name}!");
            }

            method.Invoke(this, new object[] { baseEvent });

            if (isNew)
            {
                changes.Add(baseEvent);
            }
        }

        protected void RaiseEvent(BaseEvent baseEvent)
        {
            ApplyChange(baseEvent, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var evnt in events)
            {
                ApplyChange(evnt, false);
            }
        }
    }
}
