using EckTechGames.MessageLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EckTechGames.StateMachineLibrary
{
	/// <summary>
	/// Steps are States that will trigger an OnComplete delegate when they
	/// finish the work they need to do. For steps where there isn't really any 
	/// work to do, you can set the AutoComplete to true, and it will complete
	/// itself on the next UpdateTick.
	/// </summary>
	public abstract class StateStep : StateWithMessages, IStateStep
	{
		/// <inheritdoc/>
		public StateStep(string stateID, MessageDispatcher messageDispatcher) : base(stateID, messageDispatcher)
		{
		}

		/// <summary>
		/// The OnComplete delegate that gets called once this StateStep has completed its work. State Machines
		/// are expected to set the OnComplete before entering each state. This reduces the chance for a
		/// delegate reference to cause memory to hang around.
		/// </summary>
		public Action OnComplete { get; set; }

		/// <summary>
		/// When true, this step will complete itself in the next Update frame.
		/// </summary>
		protected virtual bool ShouldAutoComplete { get { return false; } }

		/// <summary>
		/// When true, our Complete (not OnComplete) was called. We can't just rely on the OnComplete delegate
		/// being null, because we may not have had an OnComplete delegate set.
		/// </summary>
		public bool WasCompleteCalled { get; protected set; }

		/// <summary>
		/// When we enter the 
		/// </summary>
		public override void Enter()
		{
			base.Enter();

			// When we enter the state, reset our WasCompleteCalled flag.
			WasCompleteCalled = false;
		}

		/// <summary>
		/// Checks to see if the step ShouldAutoComplete and calls Complete if so.
		/// </summary>
		/// <param name="deltaTime">The amount of time that has passed since the last time Update was called</param>
		public override void Update(float deltaTime)
		{
			if (ShouldAutoComplete && !WasCompleteCalled)
			{
				Complete();
			}
		}

		/// <summary>
		/// Call this method when a StateStep completes the work it was supposed to do.
		/// </summary>
		/// <seealso cref="ShouldAutoComplete"/>
		public virtual void Complete()
		{
			WasCompleteCalled = true;
			if (ShouldReportStateTransitions)
			{
				Console.WriteLine($"Completing StateStep: {GetType().Name}[{StateID}]");
			}

			// Call our OnComplete and clear out the delegate so it only gets called once.
			OnComplete?.Invoke();
			OnComplete = null;
		}


		// ******** Below is a section of code that I'm using in the SaveGameSystem that I'm 
		// * NOTE:* currently implementing. Pulling in the entire save system and cleaning it
		// ******** up seemed like overkill for a code sample. But this part was pretty cool,
		// so I wanted to show it to you. 
		//
		// When serializing objects with an OnComplete callback, I needed a way to wire those 
		// delegates back up when we loaded the game. So I save some Uniquely identifying information 
		// to specify the object, and record the method name. Then as part of the loading process, 
		// I use reflection to get a reference to the method and hook it back up.

		/*
		/// <summary>
		/// This method collects all the data necessary to serialize itself out to disk.
		/// </summary>
		/// <param name="saveGameData">The SaveGameData object that we're populating.</param>
		public override void GetSaveGameData(BaseSaveGameData saveGameData)
		{
			base.GetSaveGameData(saveGameData);

			// Cast the saveGameData object into our specific type. 
			StateStepSaveData stateStepSaveData = saveGameData as StateStepSaveData;

			if (OnComplete != null)
			{
				Delegate[] invocationArray = OnComplete.GetInvocationList();
				List<UniqueObjectCallbackSaveData> callBackSaveDataList = new List<UniqueObjectCallbackSaveData>();

				for (int i = 0; i < invocationArray.Length; ++i)
				{
					Delegate invocation = invocationArray[i];

					// Get a reference to the target of the Invocation. They MUST be a UniquelyIdentifiedObject
					// Or we won't be able to rehydrate them when we load the game.
					IUniquelyIdentifiedObject uniqueObject = invocation.Target as IUniquelyIdentifiedObject;
					if (invocation.Target == null || uniqueObject != null)
					{
						// Populate a UniqueObjectCallbackSaveData object with the target object and name of the method.
						UniqueObjectCallbackSaveData onCompleteData = new UniqueObjectCallbackSaveData();
						onCompleteData.targetRef = UniqueObjectSaveDataHelper.GetUniqueObjectRefSaveData(uniqueObject);
						onCompleteData.methodName = invocation.Method.Name;
						callBackSaveDataList.Add(onCompleteData);
					}
					else
					{
						Console.WriteLine($"ERROR - Unknown Invocation Target[{invocation.Target}] for StateStep.OnComplete[{StateID}] Serialized Targets must be UniquelyIdentifiedObjects");
					}
				}

				// If we have data to save, then set it. Otherwise, null will be fine.
				if (callBackSaveDataList.Count > 0)
					stateStepSaveData.onCompleteSaveDataList = callBackSaveDataList;
			}
		}

		/// <summary>
		/// When we're loading the game, a blank object gets created through other parts of the system
		/// and we use the serialized data to rehydrate this object.
		/// </summary>
		/// <param name="saveGameData">The SaveGameData object that we're using to rehydrate this object</param>
		public override void ApplySaveGameData(BaseSaveGameData saveGameData)
		{
			base.ApplySaveGameData(saveGameData);

			// Cast the saveGameData object into our specific type. 
			StateStepSaveData stateStepSaveData = saveGameData as StateStepSaveData;

			// If we don't have any OnComplete data serialized, we're done and we can return early.
			OnComplete = null;
			List<UniqueObjectCallbackSaveData> onCompleteSaveDataList = stateStepSaveData.onCompleteSaveDataList;
			if (onCompleteSaveDataList == null || onCompleteSaveDataList.Count == 0)
				return;

			// Loop through our serialized OnComplete data
			for (int i = 0; i < onCompleteSaveDataList.Count; ++i)
			{
				UniqueObjectCallbackSaveData uniqueObjectCallbackSaveData = onCompleteSaveDataList[i];
				var targetObject = UniqueObjectDatabase.GetUniqueObject(uniqueObjectCallbackSaveData.targetRef);

				// Create the delegate with reflection
				var targetMethodInfo = targetObject.GetType().GetMethod(uniqueObjectCallbackSaveData.methodName);
				var createdDelegate = Delegate.CreateDelegate(typeof(Action), targetObject, targetMethodInfo);

				// Associate it with our OnComplete
				OnComplete += (Action)createdDelegate;
			}
		}
		*/
	}
}
