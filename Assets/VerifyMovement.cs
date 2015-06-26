using UnityEngine;
using System.Collections;

public class VerifyMovement : MonoBehaviour {

	
	public GameObject sourceObject;
	private Vector3 oldPosition;
	private Vector3 translateVector = new Vector3 (10.0f, 0.0f , 0.0f);

	// Use this for initialization
	void Start () {
		oldPosition = sourceObject.transform.position;
		translateVector = oldPosition + translateVector;
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 temp = sourceObject.transform.position;
		if (oldPosition.x < sourceObject.transform.position.x)
		{

			oldPosition = temp;
			print("right");
		}
		else if (oldPosition.x > sourceObject.transform.position.x)
		{
			oldPosition = temp;
			print ("left");
		}


	}
}
