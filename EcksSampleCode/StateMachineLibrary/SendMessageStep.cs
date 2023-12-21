using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// This step just sends a message when the state is entered and auto completes itself. It's
	/// useful for fire and forget actions when you don't need to wait on the results.
	/// </summary>
	/// <seealso cref="SendMessageWithResponseStep{MessageType}"/>
	public abstract class SendMessageStep<MessageType> : StateStep where MessageType : class, IMessage
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stateID">A string name for this state. Should be unique among other states in your FiniteStateMachine</param>
		/// <param name="messageDispatcher">The MessageDispatcher that this class should use.</param>
		public SendMessageStep(string stateID, MessageDispatcher messageDispatcher) : base(stateID, messageDispatcher)
		{
		}

		/// <summary>
		/// Implement this method by creating the message, and sending it off.
		/// </summary>
		/// <returns>The newly created message</returns>
		public abstract MessageType CreateMessage();

		/// <summary>
		/// The message we're going to send
		/// </summary>
		protected MessageType messageToSend = null;

		/// <summary>
		/// When we enter this state, we get the message and queue it up in the message dispatcher
		/// </summary>
		public override void Enter()
		{
			base.Enter();

			messageToSend = CreateMessage();
			MessageDispatcher.EnqueueMessage(messageToSend);
		}

		/// <summary>
		/// The send message step auto completes itself after we send the message.
		/// </summary>
		protected override bool ShouldAutoComplete => true;
	}
}
