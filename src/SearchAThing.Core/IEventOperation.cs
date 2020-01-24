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

namespace SearchAThing.Core
{

    /// <summary>
    /// Event operation behavior type describe how the past events are dispatched to the listeners.
    /// https://searchathing.com/?p=627
    /// </summary>
    public enum EventOperationBehaviorTypes
    {
        /// <summary>
        /// when listener attach its handler it will be notified only for new events.
        /// </summary>
        Normal,

        /// <summary>
        /// When listener attach its handler it will be notified automatically for the past events.
        /// </summary>
        RemindPastEvents,

        Stopped
    };

    /// <summary>
    /// Event handler operation helper.
    /// Tutorial ( http://development-annotations.blogspot.it/2015/07/c-complete-operation-event-handler.html ).
    /// </summary>    
    public interface IEventOperation<T> where T : EventArgs
    {

        /// <summary>
        /// Behavior of the event handler dispatcher when an event is fired.
        /// </summary>
        EventOperationBehaviorTypes behavior { get; }

        /// <summary>
        /// Attach to this event handler to get notified when a new event fired.
        /// If the behavior is set to "RemindPastEvent" you'll get noticed for the past events too.
        /// </summary>
        event EventHandler<T> Event;

        /// <summary>        
        /// Propagate the event to the attached listeners.
        /// <param name="sender">Reference to the object that generated this event.</param>
        /// </summary>       
        void Fire(object sender = null, T args = null);

        /// <summary>
        /// Counts how many time the event was generated.
        /// </summary>
        int FireCount { get; }

        /// <summary>
        /// Counts how many time the event was handled.
        /// </summary>
        int HandledCount { get; }

    }

    /// <summary>
    /// Event handler operation helper (with default EventArgs type as event delegate).
    /// </summary>
    public interface IEventOperation : IEventOperation<EventArgs> { }

}
