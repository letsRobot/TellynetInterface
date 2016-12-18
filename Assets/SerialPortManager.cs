using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;

public class SerialPortManager : MonoBehaviour {

	//This class is used to manage serial connections to local hardware. 
	//This will include USB, Bluetooth, and even Wifi.

	public void showPortNames() {

		//Check which OS we are using.
		int p = (int)System.Environment.OSVersion.Platform;

		//Make list to fill with serial ports.
		List<string> serial_ports = new List<string> ();
		
		//Serial Port addresses are different in Unix, so change them accordingly.
		if (p == 4 || p == 128 || p == 6) {
			string[] ttys = System.IO.Directory.GetFiles ("/dev/", "tty.*");
			//Debug.Log("Unix Ports: ");
			foreach (string dev in ttys) {
				if (dev.StartsWith ("/dev/tty")) {
					serial_ports.Add (dev);
					//Debug.Log (System.String.Format (dev));
					//Debug.Log ("Serial Count: " + serial_ports.Count);
				}
			}

		} else {

			//COM port Method for PC
			//Debug.Log("COM Ports: ");
			string[] COMports = SerialPort.GetPortNames();
			foreach (string COM in COMports) {
				serial_ports.Add(COM);
				//Debug.Log(System.String.Format(COM));
				//Debug.Log ("Serial Count: " + serial_ports.Count);
			}
		}

		//Debug.Log ("# of Serial Ports Detected: " + serial_ports.Count);

		//Looking For a specific string in port name
		for (int i = 0; i < serial_ports.Count; i++) {
		//foreach (string myPorts in serial_ports) {
			//Debug.Log ("Checking the Ports");
			string thisPort = (string)serial_ports[i];
			sendUSB(thisPort);
			if (thisPort.Contains("ArcBotics")) {
				//Debug.Log("Use this Port!");
			}
		}
	}

	void sendUSB (string usb) {
		displayText.fromUSB += usb + ("\n");
		//Debug.Log("Update USB");
	}
	
	// Use this for initialization
	void Start () {
		showPortNames();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
