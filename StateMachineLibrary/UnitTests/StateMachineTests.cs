using EckTechGames.MessageLibrary;
using EckTechGames.MessageLibrary.Exceptions;
using EckTechGames.StateMachineLibrary;
using NUnit.Framework;
using System;

namespace UnitTests
{
	public class StateMachineTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			State.ShouldReportStateTransitions = true;
			messageDispatcher = new MessageDispatcher();
		}

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
			messageDispatcher.UnsubscribeFromAllMessages();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
		}

		MessageDispatcher messageDispatcher;

		/// <summary>
		/// This test just makes sure the TimedStep works appropriately.
		/// </summary>
		[Test]
		public void TestTimedStep()
		{
			TimedStep timedStep = new TimedStep("TimedStep01", messageDispatcher, 1f);

			timedStep.Enter();

			// Wait for 0 seconds and make sure the timer hasn't completed.
			timedStep.Update(0f);
			Assert.IsFalse(timedStep.IsDelayCompleted);

			// Wait for 0.5 more seconds and make sure the timer hasn't completed.
			timedStep.Update(0.5f);
			Assert.IsFalse(timedStep.IsDelayCompleted);

			// Wait for 0.5 more seconds and make sure the timer has completed.
			timedStep.Update(0.5f);
			Assert.IsTrue(timedStep.IsDelayCompleted);
		}

		float currentValue;
		float newValue = 7;

		[Test]
		public void TestSimpleSequence()
		{
			float startingValue = 3;

			currentValue = startingValue;
			TestSequenceStateMachine testSequenceStateMachine = new TestSequenceStateMachine("Sequence01", messageDispatcher);

			testSequenceStateMachine.OnComplete = HandleSimpleSequenceComplete;

			Assert.IsNull(testSequenceStateMachine.CurrentState);
			testSequenceStateMachine.Update(1f);

			// Make sure we didn't change our currentValue yet. That should only happen after the sequence completes
			Assert.AreEqual(startingValue, currentValue);

			// Make sure we start at state 1.
			testSequenceStateMachine.Enter();
			Assert.IsNotNull(testSequenceStateMachine.CurrentState);
			Assert.AreEqual(TestSequenceStateMachine.STEP_01_ID, testSequenceStateMachine.CurrentState.StateID);

			// Wait for half a second and make sure we're still in Step01
			testSequenceStateMachine.Update(0.5f);
			Assert.AreEqual(TestSequenceStateMachine.STEP_01_ID, testSequenceStateMachine.CurrentState.StateID);

			// Wait for half a second more and make sure we're now in Step02
			testSequenceStateMachine.Update(0.5f);
			Assert.AreEqual(TestSequenceStateMachine.STEP_02_ID, testSequenceStateMachine.CurrentState.StateID);

			// Wait for two seconds and make sure we're now in Step03
			testSequenceStateMachine.Update(2f);
			Assert.AreEqual(TestSequenceStateMachine.STEP_03_ID, testSequenceStateMachine.CurrentState.StateID);

			// Wait for two seconds and make sure we're now complete
			testSequenceStateMachine.Update(3f);
			Assert.IsNull(testSequenceStateMachine.CurrentState);

			// Make sure we didn't change our currentValue yet. That should only happen after the sequence completes
			Assert.AreEqual(newValue, currentValue);
		}


		/// <summary>
		/// Sets the currentValue to 7 so we can test that the SimpleSequence called its OnComplete method.
		/// </summary>
		private void HandleSimpleSequenceComplete()
		{
			currentValue = newValue;
		}
	}
}