using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	public abstract class State : IState
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stateID">A string name for this state. Should be unique among other states in your FiniteStateMachine</param>
		/// <param name="messageDispatcher">The MessageDispatcher that this class should use.</param>
		public State(string stateID, MessageDispatcher messageDispatcher)
		{
			StateID = stateID;
			MessageDispatcher = messageDispatcher;
		}

		/// <summary>
		/// When true, we will write log messages for entering and exiting every state. In a more complex
		/// system, we might have flags for individual States/StateMachines, but a global flag will serve 
		/// us well for this sample.
		/// </summary>
		public static bool ShouldReportStateTransitions { get; set; } = false;

		/// <summary>
		/// A string that identifiest the state uniquely among other states in your FiniteStateMachine.
		/// </summary>
		public string StateID { get; protected set; }

		/// <summary>
		/// The MessageDispatcher we should use. 
		/// </summary>
		protected MessageDispatcher MessageDispatcher { get; set; }

		/// <summary>
		/// When we enter this state, check if we ShouldReportStateTransitions andwrite a log message reporting that we did.
		/// </summary>
		public virtual void Enter()
		{
			if (ShouldReportStateTransitions)
			{
				Debug.WriteLine($"Entering State: {this}");
			}
		}

		/// <summary>
		/// Nothing to do for Update yet, so leave it as an abstract method.
		/// </summary>
		/// <param name="deltaTime">The amount of time that has passed since the last time Update was called</param>
		public abstract void Update(float deltaTime);

		/// <summary>
		/// When we exit this state, check if we ShouldReportStateTransitions andwrite a log message reporting that we did.
		/// </summary>
		public virtual void Exit()
		{
			if (ShouldReportStateTransitions)
			{
				Debug.WriteLine($"Exiting State: {this}");
			}
		}

		/// <summary>
		/// Overriding ToString to have a consistent way of reporting states in logging messages
		/// </summary>
		/// <returns>The type name and StateID for ease of identifying the state.</returns>
		public override string ToString()
		{
			return $"{GetType().Name}[{StateID}]";
		}
	}
}
