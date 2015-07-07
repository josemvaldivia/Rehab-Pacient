using UnityEngine;
using System.Collections;

public class PanelController : MonoBehaviour { 
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad > 4.0) {
			transform.Translate(0, 70 * Time.deltaTime, 0);
		}
	}
}
