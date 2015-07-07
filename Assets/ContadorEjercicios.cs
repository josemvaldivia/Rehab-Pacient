using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ContadorEjercicios : MonoBehaviour {
	public int cur = 0;
	public int total = 7;
	public int serie = 1;

	// Use this for initialization
	void Start () {
	
	}

	void setText () {
		if (cur > total) {
			cur = 0;
			serie++;
		}
		Text t = gameObject.GetComponent<Text> ();
		t.text = "Serie " + serie + "\n" + cur + "/" + total;
	}

	// Update is called once per frame
	void Update () {
		setText ();
	}
}
