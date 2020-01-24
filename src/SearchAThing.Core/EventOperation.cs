#region SearchAThing.Core, Copyright(C) 2015-2016 Lorenzo Delana, License under MIT
/*
* The MIT License(MIT)
* Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
* DEALINGS IN THE SOFTWARE.
*/
#endregion

using System;
using System.Collections.Generic;

namespace SearchAThing.Core
{

    // Ref. doc: https://searchathing.com/?p=627

    public class EventOperation<T> : IEventOperation<T> where T : EventArgs
    {
        object eventHandlerLck;
        EventHandler<T> eventHandler;        

        /// <summary>
        /// This list will be populated only if behavior is set to "RemindPastEvents".
        /// </summary>
        List<Tuple<object, T>> firedEvents;

        public int FireCount { get; private set; }
        public int HandledCount { get; private set; }
        public EventOperationBehaviorTypes behavior { get; private set; }

        public event EventHandler<T> Event
        {
            add
            {
                if (behavior == EventOperationBehaviorTypes.RemindPastEvents)
                {
                    // notify past events to the listener attached after events generation
                    foreach (var x in firedEvents) { value(x.Item1, x.Item2); ++HandledCount; }
                }

                lock (eventHandlerLck)
                {
                    eventHandler += value;
                }
            }
            remove
            {
                lock (eventHandlerLck)
                {
                    eventHandler -= value;
                }
            }
        }

        public EventOperation(EventOperationBehaviorTypes _behavior = EventOperationBehaviorTypes.Normal)
        {
            eventHandlerLck = new object();
            behavior = _behavior;
            firedEvents = new List<Tuple<object, T>>();
        }

        public void Stop()
        {
            firedEvents.Clear();
            behavior = EventOperationBehaviorTypes.Stopped;
        }

        public void Fire(object sender = null, T args = null)
        {
            if (behavior == EventOperationBehaviorTypes.Stopped) return;

            if (behavior == EventOperationBehaviorTypes.RemindPastEvents)
                firedEvents.Add(new Tuple<object, T>(sender, args));

            if (eventHandler != null)
            {
                EventHandler<T> fireHandler;
                var listenerCount = 0;
                lock (eventHandlerLck)
                {
                    fireHandler = eventHandler;
                    listenerCount = fireHandler.GetInvocationList().Length;
                }
                if (fireHandler != null)
                {
                    fireHandler(sender, args);
                    HandledCount += listenerCount;
                }
            }

            ++FireCount;
        }
    }

    public class EventOperation : EventOperation<EventArgs>, IEventOperation
    {
        public EventOperation(EventOperationBehaviorTypes _behavior = EventOperationBehaviorTypes.Normal) : base(_behavior) { }
    }

}
