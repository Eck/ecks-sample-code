using System;
using System.Collections.Generic;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// A StateStep is a State that performs an action and then calls its OnComplete
	/// method. This is useful for state's that form a sequence of States (like most 
	/// table-top turn-based games).
	/// </summary>
	public interface IStateStep : IState
	{
		/// <summary>
		/// The OnComplete gets called when the Step accomplishes its task.
		/// </summary>
		Action OnComplete { get; set; }
	}
}
