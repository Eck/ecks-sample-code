using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// This class manages several distinct states and encapsulates the logic of changing between 
	/// them. For my current project's purposes, almost all my FSM's are sequences, so I inherited from
	/// StateStep instead of State. It's safe to ignore the OnComplete delegate if your FiniteStateMachine
	/// doesn't need it.
	/// </summary>
	public abstract class FiniteStateMachine : StateStep
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stateID">A string name for this state. Should be unique among other states in your FiniteStateMachine</param>
		/// <param name="messageDispatcher">The MessageDispatcher that this class should use.</param>
		public FiniteStateMachine(string stateID, MessageDispatcher messageDispatcher) : base(stateID, messageDispatcher)
		{
		}

		/// <summary>
		/// The current state that this state machine is in. Can be null when we aren't in 
		/// a state yet, or when we've completed our state machine.
		/// </summary>
		public IState CurrentState { get; protected set; }

		/// <summary>
		/// Internal dictionary of state ID's to the actual states. Put any states you create
		/// into this dictionary
		/// </summary>
		protected Dictionary<string, IState> _stateDict = new Dictionary<string, IState>();

		/// <summary>
		/// Registers a state in our dictionary of StateIDs to states. 
		/// </summary>
		/// <param name="state">The staet to register</param>
		/// <exception cref="InvalidOperationException">Thrown when a state in our dictionary already has that same StateID</exception>
		protected void RegisterState(IState state)
		{
			// If we already have a state with the same ID, throw an exception
			if (_stateDict.ContainsKey(state.StateID))
			{
				throw new InvalidOperationException($"A state with the StateID[{state.StateID}] already exists. You can't register a new state with that ID.");
			}

			_stateDict[state.StateID] = state;
		}

		/// <summary>
		/// This FiniteStateMachine can also be a State for a containing FSM. 
		/// </summary>
		public override void Enter()
		{
			base.Enter();
		}

		/// <summary>
		/// Update is intended to be called once per frame. And the CurrentState's update method
		/// is called by us. This is not a MonoBehaviour, so a containing MonoBehaviour should
		/// call this method.
		/// </summary>
		public override void Update(float deltaTime)
		{
			CurrentState?.Update(deltaTime);
		}

		/// <summary>
		/// Changing State will exit the CurrentState, set the new state, and then Enter the new state.
		/// Null is a valid state to change to.
		/// </summary>
		/// <param name="newState">The new state we're changing into. Null is valid.</param>
		public virtual void ChangeState(IState newState)
		{
			// If newState is specified, make sure they're setting this state machine to a State we actually 
			// contain inside our state machine.
			if (newState != null)
			{
				IState existingState;
				_stateDict.TryGetValue(newState.StateID, out existingState);

				if (existingState == null || existingState != newState)
				{
					throw new ArgumentException($"The passed in state {newState} does not exist inside this state machine.");
				}
			}

			// Exit the current state
			CurrentState?.Exit();

			// Set the new state and enter it.
			CurrentState = newState;
			CurrentState?.Enter();
		}

		/// <summary>
		/// Looks up the state by the specified stateID and returns it, or null if you ask for 
		/// a state that doesn't exist.
		/// </summary>
		/// <param name="stateID">The ID of the state</param>
		/// <returns>The requested state or null if it doesn't exist</returns>
		/// <exception cref="ArgumentException">If a state with the specified StateID doesn't exist</exception>
		public virtual IState GetStateByID(string stateID)
		{
			if (string.IsNullOrEmpty(stateID))
				return null;

			IState state = null;
			_stateDict.TryGetValue(stateID, out state);

			if (state == null)
			{
				throw new ArgumentException($"The passed in stateID[{stateID}] does not exist inside this state machine.");
			}

			return state;
		}


		/// <summary>
		/// Looks up the state by stateID and changes to it.
		/// </summary>
		/// <param name="stateID">The ID of the state we want to change to.</param>
		/// <exception cref="ArgumentException">If a state with the specified StateID doesn't exist</exception>
		public virtual void ChangeStateByID(string stateID)
		{
			IState state = GetStateByID(stateID);
			ChangeState(state);
		}

		/// <summary>
		/// When this FiniteStateMachine runs it's course, we'll Complete ourselves and exit
		/// the state.
		/// </summary>
		public override void Complete()
		{
			ChangeState(null);

			base.Complete();

			Exit();
		}
	}
}
