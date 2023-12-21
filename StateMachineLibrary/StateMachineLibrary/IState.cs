using System;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// This interface defines the methods for a distinct state.
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// A string that identifiest the state uniquely among other states in your FiniteStateMachine.
		/// </summary>
		string StateID { get; }

		/// <summary>
		/// The Enter method gets called when this state is entered. Initialization
		/// code, or code that runs once should go here.
		/// </summary>
		void Enter();

		/// <summary>
		/// The Update method gets called once per frame so long as the StateMachine is in this 
		/// state. Code that makes decisions for the AI, or code that polls for user input should
		/// go here.
		/// </summary>
		/// <param name="deltaTime">The amount of time that has passed since the last time Update was called</param>
		void Update(float deltaTime);

		/// <summary>
		/// The exit method gets calle when this state is exited. Clean up/tear down code 
		/// should go here.
		/// </summary>
		void Exit();
	}
}
