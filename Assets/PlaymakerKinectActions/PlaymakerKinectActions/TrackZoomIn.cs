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
	[Tooltip("Allows you to track the ZoomIn gesture from the Kinect.")]
	
	public class TrackZoomIn : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the gesture progress.")]
		public FsmFloat gestureProgress;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the zoom factor.")]
		public FsmFloat zoomFactor;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;
		
//		// for debugging purposes only
//		public FsmGameObject zoomedGameObj;
		
		[Tooltip("Custom event to be sent on zoom in detection.")]
		public FsmEvent zoomDetectedEvent;
		
		private KinectManager manager;
		private bool isGestureInitialized;
		

		// called when the state becomes active
		public override void OnEnter()
		{			
			gestureProgress.Value = 0f;
			zoomFactor.Value = 0f;
			
			isGestureInitialized = false;
		}
		
		// called before leaving the current state
		public override void OnExit ()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && 
				isGestureInitialized && manager.GetPlayer1ID() > 0)
			{
				uint userId = manager.GetPlayer1ID();
				manager.DeleteGesture(userId, KinectGestures.Gestures.ZoomIn);
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
				
				if(!manager.IsGestureDetected(userId, KinectGestures.Gestures.ZoomIn))
				{
					manager.DetectGesture(userId, KinectGestures.Gestures.ZoomIn);
					isGestureInitialized = true;
				}

				gestureProgress.Value = manager.GetGestureProgress(userId, KinectGestures.Gestures.ZoomIn);
				
				if(gestureProgress.Value > 0.1f)
				{
					Vector3 vScreenPos = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.ZoomIn);
					
					float oldZoomFactor = zoomFactor.Value;
					zoomFactor.Value = vScreenPos.z;
					
//					if(zoomedGameObj.Value)
//					{
//						Vector3 vScale = new Vector3(vScreenPos.z, vScreenPos.z, vScreenPos.z);
//						zoomedGameObj.Value.transform.localScale = vScale;
//					}

					if(oldZoomFactor != zoomFactor.Value)
					{
						Fsm.Event(zoomDetectedEvent);
					}
				}
			}
		}
	}
}