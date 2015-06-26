using UnityEngine;
using System.Collections;

public class VerifyMovement : MonoBehaviour {

	
	public GameObject sourceObject;
	public GameObject deleteObject;
	public int index = 0;
	private Vector3 oldPosition;
	private Vector3 translateVector = new Vector3 (3.0f, 0.0f , 0.0f);
	public InteractionWrapper.InteractionHandEventType handEvent = InteractionWrapper.InteractionHandEventType.None;
	public GameObject text;
	// Use this for initialization
	void Start () {
		oldPosition = sourceObject.transform.position;
		translateVector = oldPosition + translateVector;
		print (translateVector);

	
	}
	
	// Update is called once per frame
	void Update () {
		handEvent = (InteractionWrapper.InteractionHandEventType)InteractionWrapper.GetRightHandEvent();
		Vector3 temp = sourceObject.transform.position;
		text.transform.position = sourceObject.transform.position;
		if (handEvent == InteractionWrapper.InteractionHandEventType.Release) {

			if (sourceObject.transform.position.x < translateVector.x)
			{
				sourceObject.transform.position = oldPosition;
				print ("nee");
			}
			else
			{
				Destroy(deleteObject);
				Application.LoadLevel(index);
				print ("ooo");
			}
		}


	}
}
