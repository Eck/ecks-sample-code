using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.MessageLibrary
{
	/// <summary>
	/// A simple struct that can keep track of the stack trace for queued messages when requested.
	/// </summary>
	/// <seealso cref="MessageDispatcher.RecordStackTraceForDebugging"/>
	public struct QueuedMessage
	{
		/// <summary>
		/// The message that was queued
		/// </summary>
		public IMessage message;

		/// <summary>
		/// The StackTrace when the message was queued (if MessageDispatcher.RecordStackTraceForDebugging is set to true)
		/// </summary>
		public StackTrace queuedStackTrace;
	}
}
