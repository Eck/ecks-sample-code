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
	}
}