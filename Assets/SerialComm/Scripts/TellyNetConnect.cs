using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class TellyNetConnect : MonoBehaviour {

	int messageId = 0;

	// Use this for initialization
	IEnumerator Start () {

		// Tellynet Server
		string tellynetServer = "ec2-54-191-54-225.us-west-2.compute.amazonaws.com";
		string tellynetPort = ":3000";
		string tellynetSocketProtocol = "ws://";

		// Robot
		string portName = "/dev/tty.ArcBotics-DevB";
		int baudRate = 9600;
		int delayBeforeReconnecting = 1000;
		int maxUnreadMessages = 5;

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
				serialThread.SendSerialMessage (reply);
				Debug.Log (reply);
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
