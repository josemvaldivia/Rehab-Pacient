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
	[Tooltip("Allows you to track the Wheel gesture from the Kinect.")]
	
	public class TrackWheel : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the gesture progress.")]
		public FsmFloat gestureProgress;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the wheel angle.")]
		public FsmFloat wheelAngle;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;
		
//		// for debugging purposes only
//		public FsmGameObject wheelGameObj;
		
		[Tooltip("Custom event to be sent on wheel detection.")]
		public FsmEvent wheelDetectedEvent;
		
		private KinectManager manager;
		private bool isGestureInitialized;
		

		// called when the state becomes active
		public override void OnEnter()
		{			
			gestureProgress.Value = 0f;
			wheelAngle.Value = 0f;
			
			isGestureInitialized = false;
		}
		
		// called before leaving the current state
		public override void OnExit ()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && 
				isGestureInitialized && manager.GetPlayer1ID() > 0)
			{
				uint userId = manager.GetPlayer1ID();
				manager.DeleteGesture(userId, KinectGestures.Gestures.Wheel);
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
				
				if(!manager.IsGestureDetected(userId, KinectGestures.Gestures.Wheel))
				{
					manager.DetectGesture(userId, KinectGestures.Gestures.Wheel);
					isGestureInitialized = true;
				}

				gestureProgress.Value = manager.GetGestureProgress(userId, KinectGestures.Gestures.Wheel);
				
				if(gestureProgress.Value > 0.1f)
				{
					Vector3 vScreenPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.Wheel);
					
					float oldWheelAngle = wheelAngle.Value;
					wheelAngle.Value = vScreenPos.z;
					
//					if(wheelGameObj.Value)
//					{
//						Vector3 vRot = new Vector3(0f, 0f, -vScreenPos.z);
//						wheelGameObj.Value.transform.localRotation = Quaternion.Euler(vRot);
//					}

					if(oldWheelAngle != wheelAngle.Value)
					{
						Fsm.Event(wheelDetectedEvent);
					}
				}
			}
		}
	}
}