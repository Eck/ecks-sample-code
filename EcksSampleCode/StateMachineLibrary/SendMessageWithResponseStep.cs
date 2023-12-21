using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// This step just sends a message and waits for the required response before it completes
	/// istelf. It's useful for steps that require User Input for a decision point. I also plan to use 
	/// this for AI decision points when it thinks about what it should do next.
	/// </remarks>
	public abstract class SendMessageWithResponseStep<MessageType> : SendMessageStep<MessageType> where MessageType : class, IMessageWithCallback
	{
		/// <inheritdoc/>
		public SendMessageWithResponseStep(string stateID, MessageDispatcher messageDispatcher) : base(stateID, messageDispatcher)
		{
		}

		/// <summary>
		/// Flag that gets set to true when the sent message has its complete called.
		/// </summary>
		protected bool workCompleted = false;

		/// <summary>
		/// Once our workCompleted flag is set we can auto complete.
		/// </summary>
		protected override bool ShouldAutoComplete { get { return workCompleted; } }

		/// <summary>
		/// Reset our workComplted flag before entering the state
		/// </summary>
		public override void Enter()
		{
			workCompleted = false;

			base.Enter();
		}

		/// <summary>
		/// When our message gets handled we can set our workCompleted flag.
		/// </summary>
		/// <param name="message">The message that was sent</param>
		protected virtual void HandleWorkCompleted(IMessage message)
		{
			workCompleted = true;
		}
	}
}
