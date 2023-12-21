using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
	public class SimpleMessageWithInt : IMessage
	{
		public int someInt;
	}

	public class ThrowExceptionMessage : IMessage
	{
	}
}
