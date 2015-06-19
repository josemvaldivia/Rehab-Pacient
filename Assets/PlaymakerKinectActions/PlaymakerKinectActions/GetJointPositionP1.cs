using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect Actions")]
	[Tooltip("Allows you to get a joint position of Player 1 from the Kinect.")]
	
	public class GetJointPositionP1 : FsmStateAction
	{
		public enum NuiSkeletonPositionIndex : int
		{
			HipCenter = 0,
			Spine,
			ShoulderCenter,
			Head,
			ShoulderLeft,
			ElbowLeft,
			WristLeft,
			HandLeft,
			ShoulderRight,
			ElbowRight,
			WristRight,
			HandRight,
			HipLeft,
			KneeLeft,
			AnkleLeft,
			FootLeft,
			HipRight,
			KneeRight,
			AnkleRight,
			FootRight
		}
		
		// Setup to use the enum udpate
		[RequiredField]
		[Tooltip("Which joint you want to track.")]
		public NuiSkeletonPositionIndex kinectJoint;
		
		// store the name of the gesture called
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the position of the joint selected.")]
		public FsmVector3 jointPosition;
		
		private KinectManager manager;
		private int kinectJointIndex;
		
		
		// called when the state becomes active
		public override void OnEnter()
		{			
			kinectJointIndex = (int)kinectJoint;
			getKinectJointPos();
		}
		
		// called before leaving the current state
		public override void OnExit ()
		{
		}
		
		public override void OnUpdate()
		{
			//if (updateCall == PlayMakerUpdateCallType.Update)
			{
				getKinectJointPos();			
			}
		}
		
		// Update is called once per frame
		private void getKinectJointPos()
		{		
			if(manager == null)
			{
				manager = KinectManager.Instance;
			}
			
			if(manager != null && KinectManager.IsKinectInitialized() && manager.GetPlayer1ID() > 0)
			{
				uint userId = manager.GetPlayer1ID();
				jointPosition.Value = manager.GetJointPosition(userId, kinectJointIndex);
			}
		}
	}
}