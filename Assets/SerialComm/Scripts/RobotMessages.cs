using System;
using System.Collections.Generic;
//using SimpleJSON;
using UnityEngine;


//Basic structure for elements containing a chat message
public struct RobotChatMessage {

	public string user; //user will eventually need user type
	public string message; //message is the text input from the user, unless it's a command.
	//TODO: messages should all be messages, except we assign a message type to them in the future.
	public bool isCommand; //Is this message a command? 
	public bool isExecuting; //Is it executing?
}

//Message parameters are defined and set here to determine how they are displayed on the GUI
public struct InternalRobotMessage {


	//Set these values for a normal chat message.
	public InternalRobotMessage(string user, string message)
	{
		this.user          = user;
		this.message       = message;
		commandDescription = "";
		commandId          = 0;
		isCommand          = false;
		isExecuting        = false;
		newMessage         = false;
	}
	
	//Set these values if the message is a command
	public InternalRobotMessage(string user, string message, string commandDescription, int commandId)
	{
		this.user               = user;
		this.message            = message;
		this.commandDescription = commandDescription;
		this.commandId          = commandId;
		isCommand               = true;
		isExecuting             = false;
		newMessage              = false;
	}
	
	//These values are set by InternalRobotMessage method above
	public string user;
	public string message;
	public string commandDescription;
	public int commandId;
	public bool isCommand;
	public bool isExecuting;
	public bool newMessage;
};

//Gets messages from another source over the network via TCP Sockets
//Class uses RobotMessageReceiver & RobotMEssageSender interfaces from RobotConnection.cs
public class RobotMessages : MonoBehaviour, RobotMessageReceiver, RobotMessageSender
{

	IList<InternalRobotMessage> chatMessages = new List<InternalRobotMessage>();
	IDictionary<string, string> variables    = new Dictionary<string, string>();
	RobotConnection connection;
	public RobotMessages(string server, int port)
	{
		connection = new RobotConnection(server, port, this); 
	}

	public void SetServer(string server, int port)
	{
		connection.SetServer(server, port);
	}

	public void Stop()
	{
		connection.Stop();
	}
	
	//Message package is assembled from string
	public void NewMessage(string message)
	{
		Debug.Log ("You hit here");
		Debug.Log(message);
		AddMessage(new InternalRobotMessage ("", message));
		Debug.Log ("last");
	}

	int maxMessageNumber = 100;
	public void SetMaximumNumberOfMessages(int maxMessageNumber)
	{
		this.maxMessageNumber = maxMessageNumber;

		TrimChatMessages();
	}

	//Package up the components of the message into a List
	//TODO: This should not be directly referencing Constants, this class should be as encapsulated as possible.

	object chatMessagesLock = new object();
	public IList<RobotChatMessage> GetChatMessages()
	{
		var copyOfChatMessages = new List<RobotChatMessage>();
		lock(chatMessagesLock)
		{
			Debug.Log ("Made it to chat messages");
			Debug.Log (chatMessages.Count);
			foreach(var message in chatMessages)
			{
				var chatMessage         = new RobotChatMessage();
				chatMessage.user        = message.user;
				chatMessage.message     = message.message;
				chatMessage.isCommand   = message.isCommand;
				chatMessage.isExecuting = message.isExecuting;

				copyOfChatMessages.Add(chatMessage);
			}
		}

		return copyOfChatMessages;
	}

	//Get specific variables from broadcast source related to the robot.
	object variablesLock = new object();
	public IDictionary<string, string> GetVariables()
	{
		lock(variablesLock)
		{
			return new Dictionary<string, string>(variables);
		}
	}

	IList<RobotCommand> commands = new List<RobotCommand>();
	object commandsLock  = new object();

	public IList<RobotCommand> GetCommands()
	{
		lock(commandsLock)
		{
			IList<RobotCommand> returnCommands = commands;
			commands = new List<RobotCommand>();
			return returnCommands;
		}
	}

	public void SendMessage(string message)
	{
		connection.SendMessage(message);
	}

	void AddMessage(InternalRobotMessage message)
	{
		lock(chatMessagesLock)
		{
			chatMessages.Add(message);
		}

		TrimChatMessages();
	}

	void AddCommand(InternalRobotMessage internalRobotMessage)
	{
		lock(commandsLock)
		{
			var command = new RobotCommand(internalRobotMessage.commandDescription, internalRobotMessage.message);
			commands.Add(command);
		}
	}

	void SetCommandIsExecuting(int commandId, bool isExecuting)
	{
		lock(commandsLock)
		{
			for(int i = 0; i < chatMessages.Count; i++)
			{
				var message = chatMessages[i];

				if(message.commandId == commandId)
					message.isExecuting = isExecuting;
				else
					message.isExecuting = false;

				chatMessages[i] = message;
			}
		}
	}

	void SetVariable(string variable, string value)
	{
		lock(variablesLock)
		{
			variables[variable] = value;
		}
	}

	void TrimChatMessages()
	{
		lock(chatMessagesLock)
		{
			while(chatMessages.Count > maxMessageNumber)
				chatMessages.RemoveAt(0);
		}
	}

	
	// 20160603 rtharp
	// moved out to RobotStuff, so we can have multiple connecctions
	// but only one set of chat & variable
	// since we only have the one display of them
	//IList<InternalRobotMessage> chatMessages = new List<InternalRobotMessage>();
	//IDictionary<string, string> variables    = new Dictionary<string, string>();
}
