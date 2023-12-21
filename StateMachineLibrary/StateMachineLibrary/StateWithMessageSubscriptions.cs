using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// Abstract game state class that subscribes to messages when the state is entered, and
	/// unsubscribes from the messages when we exit this state.
	/// </summary>
	public abstract class StateWithMessages : State
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stateID">A string name for this state. Should be unique among other states in your FiniteStateMachine</param>
		/// <param name="messageDispatcher">The MessageDispatcher that this class should use.</param>
		public StateWithMessages(string stateID, MessageDispatcher messageDispatcher) : base (stateID, messageDispatcher)
		{
		}

		/// <summary>
		/// Says whether or not this state is subscribed to its messages.
		/// </summary>
		/// <returns>Returns true when this state is still subscribed to its messages.</returns>
		public bool IsSubscribed { get; private set; }

		/// <summary>
		/// When we enter this state, subscribe to our messages.
		/// </summary>
		public override void Enter()
		{
			base.Enter();

			SubscribeToMessages(shouldSubscribe: true);
		}

		// Still nothing to do for Update so don't do anything.
		//public void Update(float deltaTime) { }  

		/// <summary>
		/// When we exit this state, we unsubscribe from our messages
		/// </summary>
		public override void Exit()
		{
			base.Exit();

			SubscribeToMessages(shouldSubscribe: false);
		}

		/// <summary>
		/// Implementers of this class need to define all the messages they subscribe to. When
		/// shouldSubscribe is true, we'll subscribe to the messages. When shouldSubscribe is 
		/// false, we'll unsubscribe from those same messages.
		/// </summary>
		/// <remarks>
		/// I prefer implementing a method like this rather than separate Subscribe/Unsubscrie
		/// methods.The reason why is if they were separate methods, sometimes you'd add a subscription
		/// into the Subscribe method but forget to add the same line to the Unsubscribe method. Implementing
		/// with one method, you're guaranteed to subscribe/unsubscribe ALL of your messages.
		/// </remarks>
		/// <param name="shouldSubscribe">Whether or not we should subscribe to the messages.</param>
		public virtual void SubscribeToMessages(bool shouldSubscribe)
		{
			IsSubscribed = shouldSubscribe;
		}
	}
}
