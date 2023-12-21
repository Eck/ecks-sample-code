using EckTechGames.MessageLibrary;
using EckTechGames.StateMachineLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests.StateMachineTestClasses
{
	public class MockPlaySoundEffectMessage : IMessage
	{
		public MockPlaySoundEffectMessage(string soundEffectID)
		{
			SoundEffectID = soundEffectID;
		}

		public string SoundEffectID { get; protected set; }
	}

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
