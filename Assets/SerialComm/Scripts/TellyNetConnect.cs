using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;

public class TellyNetWrapper : MonoBehaviour { 

	public RobotMessages robotMessages;

	public TellyNetWrapper(string server, int port) {
		robotMessages = new RobotMessages (server, port);
	}
}

public class TellyNetConnect : MonoBehaviour {

	TellyNetWrapper tw;
	SerialThread serialThread;
	Thread thread;
			
	// Use this for initialization
	IEnumerator Start () {

		// Connection to Tellynet Server
		//string tellynetServer = "ec2-54-191-54-225.us-west-2.compute.amazonaws.com";
		string tellynetServer = "127.0.0.1";
		int tellynetPort = 3000;

		// Serial connection to Robot
		//string portName = "/dev/cu.usbmodem12341";
		string portName = "/dev/tty.ArcBotics-DevB";
		int baudRate = 9600;
		int delayBeforeReconnecting = 1000;
		int maxUnreadMessages = 5;

		tw = new TellyNetWrapper ("127.0.0.1", 8000);
		// Connect to the robot and start the serial thread
		serialThread = new SerialThread(portName, baudRate, delayBeforeReconnecting, maxUnreadMessages);
		thread = new Thread(new ThreadStart(serialThread.RunForever));
		thread.Start();	

		yield return 0; 
	}

	void Update() {
		var messages = tw.robotMessages.GetChatMessages();
		string botReply = serialThread.ReadSerialMessage ();
		if (messages.Count > 0) {
			string message = messages [0].message;
			Debug.Log (message);
			serialThread.SendSerialMessage (message);
		}

		if (botReply != null) {
			Debug.Log("Bot replied: " + botReply);
		}
	}
}
