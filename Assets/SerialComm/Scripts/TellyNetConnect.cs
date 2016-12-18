using UnityEngine;
using System.Collections;
using System;
using System.Threading;
using System.Collections.Generic;
using SimpleJSON;

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
		WebSocket w = new WebSocket(new Uri(tellynetSocketProtocol + tellynetServer + tellynetPort));
		yield return StartCoroutine(w.Connect());

		// Start Message Loop
		while (true)
		{
			string reply = w.RecvString();
			string botReply = serialThread.ReadSerialMessage ();


			if (reply != null) {

				if (reply.GetType () == typeof(string)) {
					try { SimpleJSON.JSONNode msg = SimpleJSON.JSON.Parse(reply);
						if (msg["bits"].Value != "") {
							Debug.Log (msg["bits"].Value);
						}
						toRobot = msg["message"].Value;
						Debug.Log(msg["username"].Value);
					} 	catch(KeyNotFoundException) {}
					
				}

				serialThread.SendSerialMessage (toRobot);
				Debug.Log (toRobot);
			}
			if (w.error != null) {
				Debug.LogError ("Error: "+w.error);
				break;
			}

			if (botReply != null) {
				Debug.Log("Bot replied: " + botReply);
			}

			yield return 0;
		}
		w.Close();
	}
}
