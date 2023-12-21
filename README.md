# Eck's Sample Code
Some potential employers are interested in code samples, so this repo will contain mine. I'm currently working on a turn-based strategy game in my spare time so I decided to pull out the messaging system and state machine into this repo to show them off.

Make sure to take a look at the ⭐ Interesting Bits ⭐ sections if you don't have time to do a deep dive into the repo.

## Messaging Library
This assembly contains interfaces for Messages, and a MessageDispatcher class that allows users to subscribe and publish messages.

![Message Library Class Diagram](https://github.com/Eck/ecks-sample-code/blob/main/EcksSampleCode/MessageLibrary/MessageLibraryClassDiagram.png)

### Classes/Interfaces
* **IMessage** - This empty interface provides a common way to reference messages. All user implemented messages should inherit from this.
* **IMessageWithCallback** - Inherits from IMessage - This interface has an OnComplete callback that should be called when the receiver is finished handling the message.
* **MessageDispatcher** - This class allows the user to associate MessageHandlers to specified MessageTypes and provides methods for Queuing messages or Publishing them immediately.
* **QueuedMessage** - A simple struct that can keep track of the sent message and the stack trace for queued messages when requested.
* **HandleMessageException** - This custom exception gets thrown when the MessageDipsatcher encounters an exception while handling queued messages inside its Update method.


### ⭐ Interesting Bits ⭐
* **MessageDispatcher.RecordStackTraceForDebugging** - Tracking back where a message came from in a queued system can be difficult. Setting this to true will record the StackTrace information at the time the message is queued. Since recording the StackTrace can be expensive, we default this to false.
* **MessageDispatcher.SubscribeToMessage(..., bool shouldSubscribe)** - I prefer a single Subscribe(shouldSubscribe) method as opposed to a separate Subscribe/Unsubscribe pair. I've seen too many accidental errors where someone adds a subscription, but then forgets to add the remove subscription which can result in a dangling reference/memory leak.

## State Machine Library
This assembly contains the implementation for a Finite StateMachine along with interfaces and classes for common types of states that I've encountered so far.

![State Machine Library Class Diagram](https://github.com/Eck/ecks-sample-code/blob/main/EcksSampleCode/StateMachineLibrary/StateMachineLibraryClassDiagram.png)

* **IState** - This interface defines the methods (Enter/Update/Exit) to be used by the FiniteStateMachine and provides a StateID property that should uniquely identify the State in its state machine.
* **IStateStep** - inherits from IState - A Step is a state with an OnComplete method that gets called after it completes its work. In my turn-based game, almost everything is a sequence of one state after another, so I built this system with that in mind.
* **State** - Inherits from IState - This abstract base class provides implementation for nearly all methods of the IState interface and serves as a good starting point for normal states.
* **StateWithMessages** - Inherits from State - This abstract base class is a State that also has a list of message subscriptions. When we Enter() the state, we subscribe to all our messages. When we Exit() the state, we Unsubscribe from all our messages.
* **StateStep** - Inherits from StateWithMessages - Steps are States that will trigger an OnComplete delegate when they finish the work they need to do. For steps where there isn't really any  work to do, you can set the AutoComplete to true, and it will complete itself on the next UpdateTick.
* **SendMessageStep<MessageType>** - This step just sends a message when the state is entered and auto completes itself. It's useful for fire and forget actions when you don't need to wait on the results.
  * **MessageType** - Your message must implement the IMessage interface
* **SendMessageWithResponseStep<MessageType>** - This step just sends a message and waits for the required response before it completes itself. It's useful for steps that require some logic to complete or input from a user/AI. 
  * **MessageType** - Your message must implement the IMessageWithCallback interface
* **TimedStep** - A timed state step will run for the delayTimeLimit passed in, and then complete itself. This is intended for items that need to have a time component for their actions or to enter a delay so things don't happen too fast for a user to follow. (Like waiting a second to show dice results)
* **FiniteStateMachine** - This class manages several distinct states and encapsulates the logic of changing between them. For my current project's purposes, almost all my FSM's are sequences, so I inherited from StateStep instead of State. It's safe to ignore the OnComplete delegate if your FiniteStateMachine doesn't need it.

### ⭐ Interesting Bits ⭐
* [Dev Journal](https://ecktechgames.com/2023/11/04/finite-state-machine-refactor-take-2/) - I wrote up a Developer Journal in early November about how I refactored my State Machine to make it easier to use. 
* **StateStep - lines 83+** - I included a commented out section of code that I'm using in the SaveGameSystem that I'm currently implementing. Pulling in the entire save system and cleaning it up seemed like overkill for a code sample, but this part was pretty cool, so I wanted to show it to you.
  * When serializing objects with an OnComplete callback, I needed a way to wire those delegates back up when we loaded the game. So I save some Uniquely identifying information to specify the object, and record the method name. Then as part of the loading process, I use reflection to get a reference to the method and hook it back up.
* FiniteStateMachines are also States themselves. This has allowed me to build complex, nested state machines for turn sequence logic.
* SendMessageStep<MessageType> and SendMessageWithResponseStep<MessageType> - are generic abstract classes with type constraints on the MessageType  

## Unit Tests
This assembly contains test cases that I've written up to show how the messaging system and state machine functionality works. They can also serve as a quick 'how-to-use' example if reading class comments isn't clear enough on its own.

### MessageTests
* **TestMessageSubscriptions** - This test makes sure the MessageDispatcher Subscribe and Publish methods works.
  * Publishes a message before subscribing to anything to make sure nothing bad happens
  * Subscribes and publishes a message to make sure we receive it.
  * Unsubscries from the message and publishes a message to make sure we do NOT receive it.
* **TestHandleMessageException** - This method makes sure the RecordStackTraceForDebugging flag works appropriately and records the stackTrace when set to true
  * Enqueue a message, pump the message queue, and make sure we don't receive the callstack info.
  * Turn on the extra debugging and try again. Then we SHOULD receive the extra StackTrace info.

### StateMachineTests  
* **TestSendMessageStep** - Makes sure the SendMessageStep class works
  * Creates a MockPlaySoundEffectStep
  * Enters the state, and update to trigger the message.
  * Makes sure the mock sound effect got set
  * Makes sure the message doesn't get sent twice if the step gets updated more than once
* **TestSendMessageStepWithResponse** - Makes sure the SendMessageStepWithResponse class works
  * Creates a MockRollToHitStep
  * Enters the state and updates to trigger a miss
  * Makes sure we did miss.
  * Enters the state and updates to trigger a hit
  * Makes sure we did hit.
* **TestTimedStep** - Makes sure the TimedStep class works appropriately
  * Enter the timed step with a 1 second delay.
  * Wait for 0 seconds and make sure we didn't complete.
  * Wait for 0.5 seconds and make sure we still didn't complete.
  * Wait for 0.5 more seconds and make sure we did complete.
* **TestSimpleSequence** - Makes sure a FiniteStateMachine sequence can complete itself.
  * Creates a TestSequenceStateMachine
  * Hooks up its OnComplete to change the currentValue
  * Enters the state and makes sure currentValue hasn't changed
  * Makes sure we progress through Step01, Step02, and Step03.
  * Once Step03 completes, makes sure the StateMachine completes itself.
  * Makes sure currentValue has changed to the specified value

### ⭐ Interesting Bits ⭐
* [Dev Journal](https://ecktechgames.com/2023/12/08/unit-tests-are-awesome-sometimes/) - I wrote up a developer journal on Unit Tests in early December and also talk a little bit about the SaveGame system that I'm currently working on.
* **MessageTestClasses/StateMachineTestClasses** - I created some implementations for the abstract base classes to make sure everything works as expected.
