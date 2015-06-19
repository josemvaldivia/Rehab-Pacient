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
	[Tooltip("Allows you to track hand cursor, grips and releases, recognized by the Kinect interaction manager.")]
	
	public class TrackGripRelease : FsmStateAction
	{
		[CheckForComponent(typeof(GUITexture))]
		[Tooltip("The GUITexture to be used as on-screen cursor for right hand.")]
		public FsmGameObject handCursorGuiTexture;

		// This creates an enum to determine which hand to use
		public enum KinectHand {BothHands, RightHand, LeftHand};
		[Tooltip("Select how you want to control the cursor.")]
		public KinectHand cursorControlledBy;

		[UIHint(UIHint.Variable)]
		[Tooltip("Store the flag, if the left hand is primary or not.")]
		public FsmBool isLeftHandPrimary;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the normalized screen position.")]
		public FsmVector3 normalizedPos;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the absolute screen position.")]
		public FsmVector3 screenPos;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected game object, if anything gets selected by the Grip.")]
		public FsmGameObject selectedGameObj;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selection point, if a game object gets selected by the Grip.")]
		public FsmVector3 selectionPoint;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;

		[Tooltip("Custom event to be sent on grip detection.")]
		public FsmEvent gripDetectedEvent;
		
		[Tooltip("Custom event to be sent on release detection.")]
		public FsmEvent releaseDetectedEvent;
		
		
		private InteractionManager manager;
		private bool bRightGripDetected;
		private bool bLeftGripDetected;
		
		
		// called when the state becomes active
		public override void OnEnter()
		{			
			isLeftHandPrimary.Value = false;
			
			normalizedPos.Value = Vector3.zero;
			screenPos.Value = Vector3.zero;
			
			bRightGripDetected = false;
			bLeftGripDetected = false;
		}
		
		public override void OnExit ()
		{
			if(bRightGripDetected || bLeftGripDetected)
			{
				bRightGripDetected = false;
				bLeftGripDetected = false;
				
				Fsm.Event(releaseDetectedEvent);
			}
		}
		
//		public override void OnLateUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.LateUpdate)
//			{
//				checkKinectInteractionStatus();			
//			}
//		}
//		
//		public override void OnFixedUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.FixedUpdate)
//			{
//				checkKinectInteractionStatus();			
//			}
//		}
		
		public override void OnUpdate()
		{
//			if (updateCall == PlayMakerUpdateCallType.Update)
			{
				checkKinectInteractionStatus();			
			}
		}
		
		private void checkKinectInteractionStatus()
		{		
			if(manager == null)
			{
				manager = InteractionManager.Instance;
			}
		
			if(manager != null && manager.IsInteractionInited())
			{
				if((cursorControlledBy == KinectHand.RightHand || cursorControlledBy == KinectHand.BothHands) && 
					manager.IsRightHandPrimary())
				{
					isLeftHandPrimary.Value = false;
					normalizedPos.Value = manager.GetRightHandScreenPos();
					
					screenPos.Value = new Vector3(normalizedPos.Value.x * Camera.main.pixelWidth, 
						normalizedPos.Value.y * Camera.main.pixelHeight, 0f);
					
					if(!bRightGripDetected && 
						manager.GetRightHandEvent() == InteractionWrapper.InteractionHandEventType.Grip)
					{
						bRightGripDetected = true;
						Fsm.Event(gripDetectedEvent);
					}
					
					if(bRightGripDetected && 
						manager.GetRightHandEvent() == InteractionWrapper.InteractionHandEventType.Release)
					{
						bRightGripDetected = false;
						selectedGameObj.Value = null;
						
						Fsm.Event(releaseDetectedEvent);
					}
				}
				else
				{
					if(bRightGripDetected)
					{
						bRightGripDetected = false;
						selectedGameObj.Value = null;
						
						Fsm.Event(releaseDetectedEvent);
					}
				}
				
				if((cursorControlledBy == KinectHand.LeftHand || cursorControlledBy == KinectHand.BothHands) && 
					manager.IsLeftHandPrimary())
				{
					isLeftHandPrimary.Value = true;
					normalizedPos.Value = manager.GetLeftHandScreenPos();
					
					screenPos.Value = new Vector3(normalizedPos.Value.x * Camera.main.pixelWidth, 
						normalizedPos.Value.y * Camera.main.pixelHeight, 0f);
					
					if(!bLeftGripDetected &&
						manager.GetLeftHandEvent() == InteractionWrapper.InteractionHandEventType.Grip)
					{
						bLeftGripDetected = true;
						Fsm.Event(gripDetectedEvent);
					}

					if(bLeftGripDetected && 
						manager.GetLeftHandEvent() == InteractionWrapper.InteractionHandEventType.Release)
					{
						bLeftGripDetected = false;
						selectedGameObj.Value = null;
						
						Fsm.Event(releaseDetectedEvent);
					}
				}
				else
				{
					if(bLeftGripDetected)
					{
						bLeftGripDetected = false;
						selectedGameObj.Value = null;
						
						Fsm.Event(releaseDetectedEvent);
					}
				}
				
				if(bRightGripDetected || bLeftGripDetected)
				{
					RaycastHit hit;
					Ray ray = Camera.mainCamera.ScreenPointToRay(screenPos.Value);
					//Debug.DrawRay(ray.origin, ray.direction);
					
					if(Physics.Raycast(ray, out hit))
					{
						selectedGameObj.Value = hit.collider.gameObject;
						selectionPoint.Value = hit.point;
					}
				}
				
			}
		}
	}
}