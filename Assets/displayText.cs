using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class displayText : MonoBehaviour {

	private string toText;
	public static string fromUSB;
	private Text thisText;
	public GameObject textObject;
	public Text TextObj;


	//This is just a hacky thing, I'm gonna make something better.
	void onAwake() {
		thisText = textObject.GetComponent<Text>();
		toText = thisText.text;
	}

	// Use this for initialization
	void Start () {
		toText = ("Loading USB Ports...");
		TextObj = textObject.GetComponent<Text>();
	
	}
	
	// Update is called once per frame
	void Update () {
		TextObj.text = toText;
		if (fromUSB != null) {
			TextObj.text = fromUSB;
		}
	}
}
