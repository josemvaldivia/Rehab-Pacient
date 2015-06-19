using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

/**
 * PlayMaker custom action
 * Based on code by Jonathan O'Duffy and Andrew Jones - Fantasy to Reality - http://www.fantasytoreality.com.au/
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect Actions")]
	[Tooltip("Allows you to detect the Click gesture from the Kinect")]
	
	public class DetectClick : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the gesture progress.")]
		public FsmFloat progress;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the normalized screen position.")]
		public FsmVector3 normalizedPos;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the absolute screen position.")]
		public FsmVector3 screenPos;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected game object, if anything gets selected by the Click.")]
		public FsmGameObject selectedGameObj;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selection point, if a game object gets selected by the Click.")]
		public FsmVector3 selectionPoint;
		
//		// for debugging purposes only
//		public FsmGameObject clickRayPos;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;

		[Tooltip("Custom event to be sent on click detection.")]
		public FsmEvent clickDetectedEvent;
		
		
		private KinectManager manager;
		private KinectGestures.Gestures gesture;
		private bool isGestureInitialized;
		
		
		// called when the state becomes active
		public override void OnEnter()
		{
			progress.Value = 0f;
			normalizedPos.Value = Vector3.zero;
			screenPos.Value = Vector3.zero;

			selectedGameObj.Value = null;
			selectionPoint.Value = Vector3.zero;
			
			gesture = KinectGestures.Gestures.Click;
			isGestureInitialized = false;
		}
		
		// called before leaving the current state
		public override void OnExit ()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && 
				isGestureInitialized && manager.GetPlayer1ID() > 0)
			{
				uint userId = manager.GetPlayer1ID();
				manager.DeleteGesture(userId, gesture);
			}
		}
		
//		public override void OnLateUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.LateUpdate)
//			{
//				checkGestureStatus();			
//			}
//		}
//		
//		public override void OnFixedUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.FixedUpdate)
//			{
//				checkGestureStatus();			
//			}
//		}
		
		public override void OnUpdate()
		{
//			if (updateCall == PlayMakerUpdateCallType.Update)
			{
				checkGestureStatus();			
			}
		}
		
		private void checkGestureStatus()
		{		
			if(manager == null)
			{
				manager = KinectManager.Instance;
			}
			
			if(manager != null && KinectManager.IsKinectInitialized() && manager.GetPlayer1ID() > 0)
			{
				uint userId = manager.GetPlayer1ID();

				if(!manager.IsGestureDetected(userId, gesture))
				{
					manager.DetectGesture(userId, gesture);
					isGestureInitialized = true;
				}

				if(manager.IsGestureComplete(userId, gesture, true))
				{
					progress.Value = 1f;	
					normalizedPos.Value = manager.GetGestureScreenPos(userId, gesture);
					screenPos.Value = new Vector3(normalizedPos.Value.x * Camera.main.pixelWidth, 
						normalizedPos.Value.y * Camera.main.pixelHeight, 0f);
					
					RaycastHit hit;
					Ray ray = Camera.mainCamera.ScreenPointToRay(screenPos.Value);
					//Debug.DrawRay(ray.origin, ray.direction);
					
					if(Physics.Raycast(ray, out hit))
					{
//						if(clickRayPos.Value)
//						{
//							// debug the ray position;
//							clickRayPos.Value.transform.position = hit.point;
//						}
						
						selectedGameObj.Value = hit.collider.gameObject;
						selectionPoint.Value = hit.point;
					}
					else
					{
						selectedGameObj.Value = null;
						selectionPoint.Value = Vector3.zero;
					}
					
					Fsm.Event(clickDetectedEvent);
				}
				else
				{
					progress.Value = manager.GetGestureProgress(userId, gesture);
				}
			
			}
		}
	}
}