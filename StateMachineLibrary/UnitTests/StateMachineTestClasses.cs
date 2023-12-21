using EckTechGames.MessageLibrary;
using EckTechGames.StateMachineLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
	/// <summary>
	/// A simple sequence that should progress through three timed steps.
	/// </summary>
	public class TestSequenceStateMachine : FiniteStateMachine
	{
		public TestSequenceStateMachine(string stateID, MessageDispatcher messageDispatcher) : base(stateID, messageDispatcher)
		{
			step01 = new TimedStep(STEP_01_ID, messageDispatcher, 1f);
			step02 = new TimedStep(STEP_02_ID, messageDispatcher, 2f);
			step03 = new TimedStep(STEP_03_ID, messageDispatcher, 3f);

			RegisterState(step01);
			RegisterState(step02);
			RegisterState(step03);
		}

		public const string STEP_01_ID = "Step01";
		public const string STEP_02_ID = "Step02";
		public const string STEP_03_ID = "Step03";

		protected TimedStep step01;
		protected TimedStep step02;
		protected TimedStep step03;

		/// <summary>
		/// When this FSM sequence is entered, we'll start at step 1.
		/// </summary>
		public override void Enter()
		{
			base.Enter();

			step01.OnComplete = HandleStep01Complete;
			ChangeState(step01);
		}

		/// <summary>
		/// When step01 is complete we'll proceed to step02
		/// </summary>
		private void HandleStep01Complete()
		{
			step02.OnComplete = HandleStep02Complete;
			ChangeState(step02);
		}

		/// <summary>
		/// When step02 is complete we'll proceed to step03
		/// </summary>
		private void HandleStep02Complete()
		{
			step03.OnComplete = HandleStep03Complete;
			ChangeState(step03);
		}

		/// <summary>
		/// Once Step 3 is complete, this FiniteStateMachine sequence is also complete.
		/// </summary>
		private void HandleStep03Complete()
		{
			Complete();
		}
	}
}
