using EckTechGames.MessageLibrary;
using NUnit.Framework;
using System;

namespace StateMachineTests
{
	public class MessageTests
	{
		[OneTimeSetUp]
		public void OneTimeSetup()
		{
		}

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public void TearDown()
		{
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
		}

		[Test]
		public void TestHandleMessageException()
		{
			MessageDispatcher messageDispatcher = new MessageDispatcher();

			ThrowExceptionMessage throwExceptionMessage = new ThrowExceptionMessage();

			messageDispatcher.SubscribeToMessage(typeof(ThrowExceptionMessage), HandleThrowExceptionMessage, shouldSubscribe:true);

			try
			{
				messageDispatcher.EnqueueMessage(throwExceptionMessage);
				messageDispatcher.Update();
			}
			catch(HandleMessageException ex)
			{
				Assert.IsNotNull(ex);
				Assert.IsNotNull(ex.InnerException);
				Assert.IsTrue(ex.InnerException.StackTrace.Contains("HandleThrowExceptionMessage"));

				Assert.IsNull(ex.QueuedStackTrace);
			}

		}

		private void HandleThrowExceptionMessage(IMessage obj)
		{
			throw new Exception("Testing message handlers that throw exceptions.");
		}

	}
}