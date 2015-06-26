using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextoAscende : MonoBehaviour {

	public float scrollSpeed = 100.0f;
	public float life = 5.0f;
	private Text text;

	void Awake() {
		text = GetComponent<Text> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (text.color.a > 0) {
			Vector3 tempPos = transform.position;
			tempPos.y += scrollSpeed * Time.deltaTime;
			transform.position =  tempPos;
			
			Color tempColor = text.color;
			tempColor.a -= Time.deltaTime * life;
			text.color = tempColor;
		} else {
			Destroy (gameObject);
		}
	}
}
