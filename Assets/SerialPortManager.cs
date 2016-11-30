using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;

public class SerialPortManager : MonoBehaviour {

	//This class is used to manage serial connections to local hardware. 
	//This will include USB, Bluetooth, and even Wifi.


	public static void showPortNames() {

		//First, identify the OS
		int p = (int)System.Environment.OSVersion.Platform;

		//Make list to fill with serial ports.
		List<string> serial_ports = new List<string> ();
		
		//Serial Port addresses are different on Unix, so change them accordingly.
		if (p == 4 || p == 128 || p == 6) {
			string[] ttys = System.IO.Directory.GetFiles ("/dev/", "tty.*");
			Debug.Log("Listing Serial Ports: ");
			foreach (string dev in ttys) {
				if (dev.StartsWith ("/dev/tty.*"))
					serial_ports.Add (dev);
				Debug.Log (System.String.Format (dev));
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		showPortNames();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
