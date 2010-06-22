using System;
using System.Net;

namespace Facebook.Graph
{
    /// <summary>
    /// Contains an <see cref="OpenGraphException"/> that was raised on an asynchronous callback.
    /// </summary>
    public class AsynchronousGraphExceptionEventArgs : EventArgs
    {
        internal AsynchronousGraphExceptionEventArgs(OpenGraphException causingException)
        {
            Exception = causingException;
        }

        /// <summary>
        /// The exception that was captured on the asynchronous callback thread.
        /// </summary>
        public OpenGraphException Exception { get; private set; }
    }

    /// <summary>
    /// Represents an event signature for asynchronous callback exception handling.
    /// </summary>
    /// <param name="sender">The object that initiated the event.</param>
    /// <param name="e">The exception arguments.</param>
    public delegate void AsynchronousGraphExceptionEventHandler(object sender, AsynchronousGraphExceptionEventArgs e);
}
