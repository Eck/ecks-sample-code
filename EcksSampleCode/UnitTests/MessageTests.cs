using EckTechGames.MessageLibrary;
using EckTechGames.MessageLibrary.Exceptions;
using NUnit.Framework;
using System;

namespace UnitTests
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


		protected int currentValue;

		/// <summary>
		/// This test makes sure the MessageDispatcher Subscribe and Publish methods works.
		/// </summary>
		[Test]
		public void TestMessageSubscriptions()
		{
			MessageDispatcher messageDispatcher = new MessageDispatcher();

			int startingValue = 7;
			int newValue = 3;

			currentValue = startingValue;
			SimpleMessageWithInt setCurrentValue = new SimpleMessageWithInt();
			setCurrentValue.someInt = newValue;

			// Send the message without subscribing - currentValue should NOT have changed.
			Assert.IsFalse(messageDispatcher.HasSubscriptions(typeof(SimpleMessageWithInt)));
			messageDispatcher.PublishMessage(setCurrentValue);
			Assert.AreEqual(startingValue, currentValue);

			// Subscribe and send the message again - currentValue should have changed.
			messageDispatcher.SubscribeToMessage(typeof(SimpleMessageWithInt), HandleSetCurrentValue, shouldSubscribe: true);
			Assert.IsTrue(messageDispatcher.HasSubscriptions(typeof(SimpleMessageWithInt)));
			messageDispatcher.PublishMessage(setCurrentValue);
			Assert.AreEqual(newValue, currentValue);

			// Unubscribe and send the message again - currentValue should NOT have changed.
			currentValue = startingValue;
			messageDispatcher.SubscribeToMessage(typeof(SimpleMessageWithInt), HandleSetCurrentValue, shouldSubscribe: false);
			Assert.IsFalse(messageDispatcher.HasSubscriptions(typeof(SimpleMessageWithInt)));
			messageDispatcher.PublishMessage(setCurrentValue);
			Assert.AreEqual(startingValue, currentValue);
		}

		/// <summary>
		/// Sets the MessageTests.currentValue to the value specified in the passed in message.
		/// </summary>
		/// <param name="message">The SimpleMessageWithInt that we should handle.</param>
		private void HandleSetCurrentValue(IMessage message)
		{
			SimpleMessageWithInt simpleMessageWithInt = message as SimpleMessageWithInt;
			currentValue = simpleMessageWithInt.someInt;
		}

		/// <summary>
		/// This method makes sure the RecordStackTraceForDebugging flag works appropriately and 
		/// records the stackTrace when set to true. (and does NOT record the stack trace when set to 
		/// false)
		/// </summary>
		[Test]
		public void TestHandleMessageException()
		{
			// Setup the subscriptions.
			MessageDispatcher messageDispatcher = new MessageDispatcher();
			messageDispatcher.SubscribeToMessage(typeof(ThrowExceptionMessage), HandleThrowExceptionMessage, shouldSubscribe:true);

			try
			{
				// Enqueue our message, and handle them with a manual Update tick.
				WrapEnqueueThrowExceptionMessage(messageDispatcher);
				messageDispatcher.Update();
			}
			catch(HandleMessageException handleMessageException)
			{
				// Make sure we have all the normal information.
				Assert.IsNotNull(handleMessageException);
				Assert.IsNotNull(handleMessageException.InnerException);
				Assert.IsTrue(handleMessageException.InnerException.StackTrace.Contains("HandleThrowExceptionMessage"));

				// Since RecordStackTraceForDebugging is false, we shouldn't have a QueuedStackTrace set.
				Assert.IsNull(handleMessageException.QueuedStackTrace);
			}

			try
			{
				// Turn on the extra debugging and try again.
				messageDispatcher.RecordStackTraceForDebugging = true;
				WrapEnqueueThrowExceptionMessage(messageDispatcher);
				messageDispatcher.Update();
			}
			catch (HandleMessageException handleMessageException)
			{
				// Make sure we have all the normal information.
				Assert.IsNotNull(handleMessageException);
				Assert.IsNotNull(handleMessageException.InnerException);
				Assert.IsTrue(handleMessageException.InnerException.StackTrace.Contains("HandleThrowExceptionMessage"));

				// Since RecordStackTraceForDebugging was enabled, we should have that information as well.
				Assert.IsNotNull(handleMessageException.QueuedStackTrace);
				Assert.IsTrue(handleMessageException.QueuedStackTrace.ToString().Contains("WrapEnqueueThrowExceptionMessage"));
			}
		}

		/// <summary>
		/// Just a simple wraper message so I can test for this method name in the QueuedStackTrace.
		/// </summary>
		/// <param name="messageDispatcher">The messageDispatcher to enque a ThrowExceptionMessage for</param>
		private void WrapEnqueueThrowExceptionMessage(MessageDispatcher messageDispatcher)
		{
			ThrowExceptionMessage throwExceptionMessage = new ThrowExceptionMessage();
			messageDispatcher.EnqueueMessage(throwExceptionMessage);
		}

		/// <summary>
		/// A message handler that throws an exception to trigger error cases..
		/// </summary>
		/// <param name="message">The message that was sent (ignored in this method)</param>
		private void HandleThrowExceptionMessage(IMessage message)
		{
			throw new Exception("Testing message handlers that throw exceptions.");
		}
	}
}