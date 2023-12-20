using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.MessageLibrary
{
	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="MessageDispatcher.RecordStackTraceForDebugging"/>
	public class HandleMessageException : Exception
	{
		public HandleMessageException(string message, Exception innerException, StackTrace queuedStackTrace) : base(message, innerException)
		{
			QueuedStackTrace = queuedStackTrace;
		}

		public StackTrace QueuedStackTrace { get; protected set; }
	}
}
