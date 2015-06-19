using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;
using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Kinect Actions")]
	[Tooltip("Allows you to get the depth-and-user map texture from the Kinect. NOTE That you must make sure ComputeUserMap is enabled under KinectManager-script.")]
	
	public class GetDepthMap : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the depth map texture.")]
		public FsmTexture depthTexture;
		
		private KinectManager manager;
		private Texture2D tex2d;
		
		
		// called when the state becomes active
		public override void OnEnter()
		{			
			getDepthMap();
		}
		
		// called before leaving the current state
		public override void OnExit ()
		{
		}
		
		public override void OnUpdate()
		{
			//if (updateCall == PlayMakerUpdateCallType.Update)
			{
				getDepthMap();
			}
		}
		
		// Update is called once per frame
		private void getDepthMap()
		{		
			if(manager == null)
			{
				manager = KinectManager.Instance;
			}
			
			if(manager != null && KinectManager.IsKinectInitialized())
			{
				tex2d = manager.GetUsersLblTex();
				depthTexture.Value = tex2d;
			}
		}
	}
}