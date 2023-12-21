using EckTechGames.MessageLibrary;
using EckTechGames.StateMachineLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.StateMachineTestClasses
{
	/// <summary>
	/// A message that mocks a message that might be used to play a sound effect.
	/// </summary>
	public class MockPlaySoundEffectMessage : IMessage
	{
		public MockPlaySoundEffectMessage(string soundEffectID)
		{
			SoundEffectID = soundEffectID;
		}

		public string SoundEffectID { get; protected set; }
	}

	/// <summary>
	/// A mock StateStep that might be used to play a sound effect.
	/// </summary>
	public class MockPlaySoundEffectStep : SendMessageStep<MockPlaySoundEffectMessage>
	{
		public MockPlaySoundEffectStep(string stateID, MessageDispatcher messageDispatcher, string soundEffectID) : base(stateID, messageDispatcher)
		{
			SoundEffectID = soundEffectID;
		}

		public string SoundEffectID { get; protected set; }

		public override MockPlaySoundEffectMessage CreateMessage()
		{
			return new MockPlaySoundEffectMessage(SoundEffectID);
		}
	}
}
