using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using SimpleJSON;
using WebSocketSharp;

//Twitch Keys for Reference: 
//badges, subscriber, bits, color, display-name, emotes: null, id, mod, username


public class TellyNetConnect : MonoBehaviour {

	int messageId = 0;

	// Connection to Tellynet Server
	string tellynetServer = "ec2-54-191-54-225.us-west-2.compute.amazonaws.com";
	string tellynetPort = ":3000";
	string tellynetSocketProtocol = "ws://";
	
	// Serial connection to Robot
	public string portName = "/dev/tty.ArcBotics-DevB";
	public int baudRate = 9600;
	public int delayBeforeReconnecting = 1000;
	public int maxUnreadMessages = 5;

	string toRobot;


	// Use this for initialization
	IEnumerator Start () {

		// Connect to the robot and start the serial thread
		var serialThread = new SerialThread(portName, baudRate, delayBeforeReconnecting, maxUnreadMessages);
		var thread = new Thread(new ThreadStart(serialThread.RunForever));
		thread.Start();	

		// Connect to the Tellynet web socket server
		var ws = new WebSocket (tellynetSocketProtocol + tellynetServer + tellynetPort);
		ws.Connect ();
		ws.Send ("join_#letsrobot");

		ws.OnMessage += (sender, e) => {
			Console.WriteLine ("Laputa says: " + e.Data);
			string reply = e.Data;
			if (e.IsText) {
				//The messages from TellyNet are JSON objects
				try {
					SimpleJSON.JSONNode msg = SimpleJSON.JSON.Parse (reply);

					//This is a special use case for the bitslapper, 
					//We need to develop a whole new command protocol instead.
					if (msg ["bits"].Value != "") {
						Debug.Log (msg ["bits"].Value);
						var bitAmount = int.Parse (msg ["bits"].Value);
						if (bitAmount >= 100) {
							toRobot = "bitslap";
						} else if (bitAmount <= 99 && bitAmount >= 10) {
							toRobot = "bittyslap";
						}

					}

					//example of how to grab user chat messages from TellyNet.
					Debug.Log (msg ["username"].Value + ": " + msg ["message"].Value);
				} catch (KeyNotFoundException) {
				}
				//If there is a message we can send the robot, that happens here.
				if (toRobot != null) {
					serialThread.SendSerialMessage (toRobot);
					Debug.Log (toRobot);
					toRobot = "";
				}
			}
		};
		

		// Start Message Loop
		while (true)
		{
			//Grabs messages from TellyNet

			string reply = null;
			//Grabs serial messages from the robot
			string botReply = serialThread.ReadSerialMessage ();


			/*if (reply != null) {
				//Parse the string if it's readable. 
				if (reply.GetType () == typeof(string)) {
					//The messages from TellyNet are JSON objects
					try { SimpleJSON.JSONNode msg = SimpleJSON.JSON.Parse(reply);

						//This is a special use case for the bitslapper, 
						//We need to develop a whole new command protocol instead.
						if (msg["bits"].Value != "") {
							Debug.Log (msg["bits"].Value);
							var bitAmount = int.Parse(msg["bits"].Value);
							if (bitAmount >= 100) {
								toRobot = "bitslap";
							} else if (bitAmount <= 99 && bitAmount >= 10) {
								toRobot = "bittyslap";
							}

						}

						//example of how to grab user chat messages from TellyNet.
						Debug.Log(msg["username"].Value + ": " + msg["message"].Value);
					} 	catch(KeyNotFoundException) {}
				}

				//If there is a message we can send the robot, that happens here.
				if (toRobot != null) {
					serialThread.SendSerialMessage (toRobot);
					Debug.Log (toRobot);
					toRobot = "";
				}

			}*/

			if (botReply != null) {
				Debug.Log("Bot replied: " + botReply);
			}

			yield return 0;
		}
	   ws.Close();
	}
}
