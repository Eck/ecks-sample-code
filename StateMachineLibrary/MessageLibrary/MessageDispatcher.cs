using EckTechGames.MessageLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.MessageLibrary
{
	/// <summary>
	/// This class allows the user to associate MessageHandlers to specified MessageTypes and
	/// provides methods for Queing messages or Publishing them immediately. If you're debugging
	/// a problem with a MessageQueue, make sure to set RecordStackTraceForDebugging to true.
	/// </summary>
	public class MessageDispatcher
	{
		/// <summary>
		/// Tracking back where a message came from in a queued system can be a pain. Set this to 
		/// true so the stack information can be recorded when debugging a problem. Since recording
		/// the StackTrace can be expensive, we default this to false.
		/// </summary>
		public bool RecordStackTraceForDebugging { get; set; } = false;

		/// <summary>
		/// This method allows the user to associate various messageHandler delegates to the specified messageType.
		/// Any time the MessageDispatcher receives a message of the corresponding Type, it will call the 
		/// associated messageHandler.
		/// </summary>
		/// <param name="messageType">The type of the message to associate the messageHandler with</param>
		/// <param name="messageHandler">The delegate to call when we receive a message.</param>
		/// <param name="shouldSubscribe">When true we will associate the messageType messageHandler. When false, we will remove the association</param>
		/// <remarks>I prefer a single Subscribe(shouldSubscribe) method as opposed to a Subscribe/Unsubscribe pair. I've
		/// seen too many accidental errors where someone adds a subscription, but then forgets to add the remove subscription
		/// which can result in a memory leak.</remarks>
		public void SubscribeToMessage(Type messageType, Action<IMessage> messageHandler, bool shouldSubscribe)
		{
			List<Action<IMessage>> subscriptionList; 

			// If the subscription list doesn't exist yet, create it and add it to the dictionary.
			if (!subscriptionDict.TryGetValue(messageType, out subscriptionList))
			{
				subscriptionList = new List<Action<IMessage>>();
				subscriptionDict[messageType] = subscriptionList;
			}

			if (shouldSubscribe)
			{
				subscriptionList.Add(messageHandler);
			}
			else
			{
				subscriptionList.Remove(messageHandler);
			}
		}

		/// <summary>
		/// This method checks to see if there are any subscribers for a given message type
		/// </summary>
		/// <param name="messageType">The type of message you're checking for subscribers</param>
		/// <returns>True if the passed in messageType has any subscribers</returns>
		public bool HasSubscriptions(Type messageType)
		{
			// If we have a subscription list for this MessageType
			if (subscriptionDict.TryGetValue(messageType, out List<Action<IMessage>> subscriptionList))
			{
				// Then we have subscribers if we have any subscribers
				return subscriptionList.Count > 0;
			}

			// Otherwise we don't have any subscribers
			return false;
		}

		/// <summary>
		/// This method adds the passed in message to the MessageQueue which gets processed on
		/// the next Update frame. If you need to send the message immediately, use PublishMessage
		/// instead.
		/// </summary>
		/// <param name="message"></param>
		/// <seealso cref="PublishMessage(IMessage)"/>
		public void EnqueueMessage(IMessage message)
		{
			// Queue up the message and store the stacktrace if necessary.
			QueuedMessage queuedMessage = new QueuedMessage();
			queuedMessage.message = message;

			if (RecordStackTraceForDebugging)
			{
				queuedMessage.queuedStackTrace = new StackTrace(skipFrames: 1, fNeedFileInfo: true);
			}

			messageQueue.Enqueue(queuedMessage);
		}


		/// <summary>
		/// This method should be called once per frame so that it can handle messages enqueued in the messageQueue.
		/// </summary>
		public virtual void Update()
		{
			HandleMessageQueue();
		}

		/// <summary>
		/// This method loops through all the queued messages and publishes them to the
		/// subscribers.
		/// </summary>
		protected void HandleMessageQueue()
		{
			// Only handle the messages we know about now. If new messages get queued up, 
			// those will be handled next Update()
			int messageCount = messageQueue.Count;
			while (messageCount > 0)
			{
				// Pop a message off the queue and try to handle it.
				--messageCount;
				QueuedMessage queuedMessage = messageQueue.Dequeue();
				try
				{
					PublishMessage(queuedMessage.message);
				}
				catch (Exception ex)
				{
					// If there was a problem, throw our HandleMessageException.
					HandleMessageException handleMessageException = new HandleMessageException($"MessageDispatcher encountered a problem handling messageType{queuedMessage.message.GetType()}", ex, queuedMessage.queuedStackTrace);
					throw handleMessageException;
				}
			}
		}

		/// <summary>
		/// This method immediately publishes the passed in message to all the subscribers listening for that
		/// message type.
		/// </summary>
		/// <param name="message">The message to publish</param>
		/// <seealso cref="EnqueueMessage(IMessage)"/>
		public void PublishMessage(IMessage message)
		{
			// If we have subscribers for this messageType
			List<Action<IMessage>> subscriptionList;
			if (subscriptionDict.TryGetValue(message.GetType(), out subscriptionList) || subscriptionList?.Count == 0)
			{
				// Call each listener with the passed in message.
				for (int i = 0; i < subscriptionList.Count; ++i)
				{
					Action<IMessage> subscription = subscriptionList[i];
					subscription.Invoke(message);
				}
			}
			else
			{
				Debug.WriteLine($"Message sent without listener: [{message}]");
			}
		}

		/// <summary>
		/// This method clears out all MessageHandlers for all messageTypes
		/// </summary>
		public void UnsubscribeFromAllMessages()
		{
			foreach (List<Action<IMessage>> subscriptionList in subscriptionDict.Values)
			{
				subscriptionList.Clear();
			}

			subscriptionDict.Clear();
		}

		/// <summary>
		/// Dictionary of MessageTypes to Subscribers.
		/// </summary>
		protected Dictionary<Type, List<Action<IMessage>>> subscriptionDict = new Dictionary<Type, List<Action<IMessage>>>();

		/// <summary>
		/// Queue of messages to handle
		/// </summary>
		protected Queue<QueuedMessage> messageQueue = new Queue<QueuedMessage>();
	}
}
