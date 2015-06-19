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
	[Tooltip("Allows you to detect a gesture from the Kinect. NOTE That you must make sure the gesture is listed under KinectManager Script.")]
	
	public class DetectGesture : FsmStateAction
	{
		// Allow the user to determine which gesture they would like to detect
		public enum PlaymakerGestures
		{
			None = 0,
			RaiseRightHand = 1,
			RaiseLeftHand = 2,
			Psi = 3,
			Stop = 4,
			Wave = 5,
			//Click = 6,
			SwipeLeft = 7,
			SwipeRight = 8,
			SwipeUp = 9,
			SwipeDown = 10,
			//RightHandCursor = 11,
			//LeftHandCursor = 12,
			//ZoomOut = 13,
			//ZoomIn = 14,
			//Wheel = 15,
			Jump = 16,
			Squat = 17
		}

		[RequiredField]
		[Tooltip("Which gesture you want to detect.")]
		public PlaymakerGestures kinectGesure;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the gesture progress.")]
		public FsmFloat gestureProgress;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;
		
		[Tooltip("Custom event to be sent on gesture detection.")]
		public FsmEvent gestureDetectedEvent;
		
		private KinectManager manager;
		private KinectGestures.Gestures gesture;
		private bool isGestureInitialized;
		
		
		// called when the state becomes active
		public override void OnEnter()
		{			
			gestureProgress.Value = 0f;

			gesture = (KinectGestures.Gestures) Enum.Parse(typeof(KinectGestures.Gestures), kinectGesure.ToString());
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
					gestureProgress.Value = 1f;	
					Fsm.Event(gestureDetectedEvent);
				}
				else
				{
					gestureProgress.Value = manager.GetGestureProgress(userId, gesture);
				}
			
			}
		}
	}
}