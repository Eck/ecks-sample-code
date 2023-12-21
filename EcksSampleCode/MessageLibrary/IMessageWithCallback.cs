using System;

namespace EckTechGames.MessageLibrary
{
	/// <summary>
	/// This message has an OnComplete callback that should be called when it's
	/// finished handling the message. Implementers of this method should set
	/// any required data in the original message and pass that as the parameter
	/// to the OnComplete.
	/// </summary>
	public interface IMessageWithCallback : IMessage
	{
		Action<IMessageWithCallback> OnComplete { get; }
	}
}
