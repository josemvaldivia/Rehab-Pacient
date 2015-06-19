using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

/**
 * PlayMaker custom action
 * Based on code by Jonathan O'Duffy and Andrew Jones - Fantasy to Reality - http://www.fantasytoreality.com.au/
 */
namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect Actions")]
	[Tooltip("Allows you to use either the left or right hand from kinect to control the cursor on the screen. Use this action to alternate control of the cursor between left and right hand.")]
	
	public class TrackHandCursor : FsmStateAction
	{
		[CheckForComponent(typeof(GUITexture))]
		[Tooltip("The GUITexture to be used as on-screen cursor for right hand.")]
		public FsmGameObject cursorTextureRightHand;

		[CheckForComponent(typeof(GUITexture))]
		[Tooltip("The GUITexture to be used as on-screen cursor for left hand.")]
		public FsmGameObject cursorTextureLeftHand;

		// This creates an enum to determine which hand to use
		public enum KinectHand {BothHands, RightHand, LeftHand};
		[Tooltip("Select the hand that you want to control the cursor.")]
		public KinectHand cursorControlledBy;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the normalized screen position.")]
		public FsmVector3 normalizedPos;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the absolute screen position.")]
		public FsmVector3 screenPos;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;
		
		
		private KinectManager manager;
		private bool isGestureInitialized;
		
		
		// called when the state becomes active
		public override void OnEnter()
		{			
			normalizedPos.Value = Vector3.zero;
			screenPos.Value = Vector3.zero;
			
			isGestureInitialized = false;
		}
		
		// called before leaving the current state
		public override void OnExit ()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && 
				isGestureInitialized && manager.GetPlayer1ID() > 0)
			{
				uint userId = manager.GetPlayer1ID();
				
				manager.DeleteGesture(userId, KinectGestures.Gestures.RightHandCursor);
				manager.DeleteGesture(userId, KinectGestures.Gestures.LeftHandCursor);
			}
		}
		
//		public override void OnLateUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.LateUpdate)
//			{
//				UpdateHands();			
//			}
//		}
//		
//		public override void OnFixedUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.FixedUpdate)
//			{
//				UpdateHands();			
//			}
//		}
		
		public override void OnUpdate()
		{
//			if (updateCall == PlayMakerUpdateCallType.Update)
			{
				UpdateHands();			
			}
		}
		
		// Update is called once per frame
		private void UpdateHands()
		{
			if(manager == null)
			{
				manager = KinectManager.Instance;
			}

			if (cursorControlledBy == KinectHand.BothHands)
			{
				bothHands();
			}
			else if(cursorControlledBy == KinectHand.RightHand)
			{
				rightHand();
			}
			else if(cursorControlledBy == KinectHand.LeftHand)
			{
				leftHand();
			}
			
			calculateScreenPos();
		}

		private void bothHands()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && manager.GetPlayer1ID() > 0)
			{	
				uint userId = manager.GetPlayer1ID();
				
				if(!manager.IsGestureDetected(userId, KinectGestures.Gestures.RightHandCursor))
				{
					manager.DetectGesture(userId, KinectGestures.Gestures.RightHandCursor);
					isGestureInitialized = true;
				}
				
				if(!manager.IsGestureDetected(userId, KinectGestures.Gestures.LeftHandCursor))
				{
					manager.DetectGesture(userId, KinectGestures.Gestures.LeftHandCursor);
					isGestureInitialized = true;
				}

				// cursor control
				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.RightHandCursor) >= 0.1f)
				{
					if(cursorTextureRightHand.Value)
					{
						normalizedPos.Value = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.RightHandCursor);
						cursorTextureRightHand.Value.transform.position = Vector3.Lerp(cursorTextureRightHand.Value.transform.position, normalizedPos.Value, 3 * Time.deltaTime);
					}
				}
				else if(manager.GetGestureProgress(userId, KinectGestures.Gestures.LeftHandCursor) >= 0.1f)
				{
					if(cursorTextureLeftHand.Value)
					{
						normalizedPos.Value = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.LeftHandCursor);
						cursorTextureLeftHand.Value.transform.position = Vector3.Lerp(cursorTextureLeftHand.Value.transform.position, normalizedPos.Value, 3 * Time.deltaTime);
					}
				}
			}
		}

		private void rightHand()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && manager.GetPlayer1ID() > 0)
			{	
				uint userId = manager.GetPlayer1ID();
				
				if(!manager.IsGestureDetected(userId, KinectGestures.Gestures.RightHandCursor))
				{
					manager.DetectGesture(userId, KinectGestures.Gestures.RightHandCursor);
					isGestureInitialized = true;
				}
				
				// cursor control
				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.RightHandCursor) >= 0.1f)
				{
					if(cursorTextureRightHand.Value)
					{
						normalizedPos.Value = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.RightHandCursor);
						cursorTextureRightHand.Value.transform.position = Vector3.Lerp(cursorTextureRightHand.Value.transform.position, normalizedPos.Value, 3 * Time.deltaTime);
					}
				}
			}
		}

		private void leftHand()
		{
			if(manager != null && KinectManager.IsKinectInitialized() && manager.GetPlayer1ID() > 0)
			{	
				uint userId = manager.GetPlayer1ID();

				if(!manager.IsGestureDetected(userId, KinectGestures.Gestures.LeftHandCursor))
				{
					manager.DetectGesture(userId, KinectGestures.Gestures.LeftHandCursor);
					isGestureInitialized = true;
				}
				
				if(manager.GetGestureProgress(userId, KinectGestures.Gestures.LeftHandCursor) >= 0.1f)
				{
					if(cursorTextureLeftHand.Value)
					{
						normalizedPos.Value = manager.GetGestureScreenPos(userId, KinectGestures.Gestures.LeftHandCursor);
						cursorTextureLeftHand.Value.transform.position = Vector3.Lerp(cursorTextureLeftHand.Value.transform.position, normalizedPos.Value, 3 * Time.deltaTime);
					}

				}
			}
		}
		
		private void calculateScreenPos()
		{
			screenPos.Value = new Vector3(normalizedPos.Value.x * Camera.main.pixelWidth, 
				normalizedPos.Value.y * Camera.main.pixelHeight, 0f);
		}
		
	}
}