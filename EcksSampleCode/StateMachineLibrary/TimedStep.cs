using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// A timed state step will run for the delayTimeLimit passed in, and then complete itself.
	/// This is intended for items that need to have a time component for their actions or to enter
	/// a delay so things don't happen to fast for a user to follow. (Like waiting a second to show 
	/// dice results)
	/// </summary>
	/// <seealso cref="DelayStep"/>
	public class TimedStep : StateStep
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="stateID">A string name for this state. Should be unique among other states in your FiniteStateMachine</param>
		/// <param name="messageDispatcher">The MessageDispatcher that this class should use.</param>
		/// <param name="delayTimeLimit">The TimeLimit in seconds this step should wait.</param>
		public TimedStep(string stateID, MessageDispatcher messageDispatcher, float delayTimeLimit) : base(stateID, messageDispatcher)
		{
			this.delayTimeLimit = delayTimeLimit;
		}

		/// <summary>
		/// Whether or not the delay has run its course
		/// </summary>
		public bool IsDelayCompleted { get; protected set; } = false;

		/// <summary>
		/// How long we're going to wait.
		/// </summary>
		protected float delayTimeLimit = 0f;

		/// <summary>
		/// How long we've waited so far.
		/// </summary>
		protected float delayTimeSoFar = 0f;

		///// <summary>
		///// This TimedStep will auto complete after it's time limit is reached.
		///// </summary>
		//protected override bool ShouldAutoComplete
		//{
		//	get
		//	{
		//		return delayTimeSoFar >= delayTimeLimit;
		//	}
		//}

		/// <summary>
		/// When we enter the TimedStep, reset our delayTimeSoFar and IsDelayCompleted flags 
		/// </summary>
		public override void Enter()
		{
			base.Enter();

			delayTimeSoFar = 0f;
			IsDelayCompleted = false;
		}

		/// <summary>
		/// Update will increase the time we've waited and complete itself if we're done.
		/// </summary>
		/// <param name="deltaTime">The amount of time that has passed since the last time Update was called</param>
		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);

			// If we haven't completed the delay yet
			if (!IsDelayCompleted)
			{
				// Wait the appropriate amount of time.
				delayTimeSoFar += deltaTime;

				// If we've waited long enough, complete ourselves
				if (delayTimeSoFar >= delayTimeLimit)
				{
					IsDelayCompleted = true;
					Complete();
				}
			}
		}
	}
}
