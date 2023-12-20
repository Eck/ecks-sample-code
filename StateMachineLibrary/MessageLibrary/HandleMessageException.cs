using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.MessageLibrary.Exceptions
{
	/// <summary>
	/// An exception that gets thrown when the MessageDipsatcher encounters an exception while handling
	/// queued messages inside its Update method. For extra debugging information, make sure to set
	/// MessageDispatcher.RecordStackTraceForDebugging to true.
	/// </summary>
	/// <seealso cref="MessageDispatcher.RecordStackTraceForDebugging"/>
	public class HandleMessageException : Exception
	{
		public HandleMessageException(string message, Exception innerException, StackTrace queuedStackTrace) : base(message, innerException)
		{
			QueuedStackTrace = queuedStackTrace;
		}

		/// <summary>
		/// The StackTrace when the message was queued (if MessageDispatcher.RecordStackTraceForDebugging is set to true)
		/// Check the innerException.StackTrace to see where the exception was thrown.
		/// </summary>
		public StackTrace QueuedStackTrace { get; protected set; }
	}
}
