using EckTechGames.MessageLibrary;
using EckTechGames.StateMachineLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.StateMachineTestClasses
{
	/// <summary>
	/// A message that mocks a message that might be used to trigger some game logic to roll a chance to hit.
	/// </summary>
	public class MockRollToHitMessage : IMessageWithCallback
	{
		public MockRollToHitMessage(Action<IMessageWithCallback> onComplete)
		{
			OnComplete = onComplete;
		}

		public bool IsHit { get; set; }

		public Action<IMessageWithCallback> OnComplete { get; protected set; }
	}

	/// <summary>
	/// A mock StateStep that might be used to trigger a roll a chance to hit.
	/// </summary>
	public class MockRollToHitStep : SendMessageWithResponseStep<MockRollToHitMessage>
	{
		public MockRollToHitStep(string stateID, MessageDispatcher messageDispatcher) : base(stateID, messageDispatcher)
		{
		}

		public bool IsHit { get; protected set; }

		public override void Enter()
		{
			IsHit = false;

			base.Enter();
		}

		public override MockRollToHitMessage CreateMessage()
		{
			return new MockRollToHitMessage(HandleWorkCompleted);
		}

		protected override void HandleWorkCompleted(IMessage message)
		{
			base.HandleWorkCompleted(message);

			MockRollToHitMessage mockRollToHitMessage = message as MockRollToHitMessage;
			IsHit = mockRollToHitMessage.IsHit;
		}
	}
}
