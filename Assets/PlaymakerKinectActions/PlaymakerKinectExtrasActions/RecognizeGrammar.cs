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
	[Tooltip("Allows you to detect grammar phrases, recognized by the Kinect speech manager.")]
	
	public class RecognizeGrammar : FsmStateAction
	{
		[Tooltip("Set the grammar file name.")]
		public FsmString GrammarFileName;
		
		[Tooltip("Set the grammar language (en-US by default)")]
		public FsmInt LanguageCode;
		
		[Tooltip("Expected phrase tag, if there is any specific one.")]
		public FsmString expectedPhraseTag;
		
//		public enum PlayMakerUpdateCallType {Update,LateUpdate,FixedUpdate};
//		[Tooltip("Allow the user to determine which update to use.")]
//		public PlayMakerUpdateCallType updateCall;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Variable to store the phrase tag recognized.")]
		public FsmString phraseTagRecognized;
		
		[Tooltip("Custom event to be sent on grammar phrase detection.")]
		public FsmEvent phraseDetectedEvent;
		
		private SpeechManager manager;
		private bool isGrammarSet;
		

		// called when the state becomes active
		public override void OnEnter()
		{			
			phraseTagRecognized.Value = String.Empty;
		}
		
//		public override void OnLateUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.LateUpdate)
//			{
//				checkSpeechRecognizerStatus();			
//			}
//		}
//		
//		public override void OnFixedUpdate()
//		{
//			if (updateCall == PlayMakerUpdateCallType.FixedUpdate)
//			{
//				checkSpeechRecognizerStatus();			
//			}
//		}
		
		public override void OnUpdate()
		{
//			if (updateCall == PlayMakerUpdateCallType.Update)
			{
				checkSpeechRecognizerStatus();			
			}
		}
		
		private void checkSpeechRecognizerStatus()
		{		
			if(manager == null)
			{
				manager = SpeechManager.Instance;
			}
			
			if(manager != null && manager.IsSapiInitialized())
			{
				if(!isGrammarSet && GrammarFileName.Value != String.Empty)
				{
					isGrammarSet = true;
					
					int langCode = LanguageCode.Value != 0 ? LanguageCode.Value : 1033;
					int rc = SpeechWrapper.LoadSpeechGrammar(GrammarFileName.Value, (short)langCode);
					
			        if (rc < 0)
			        {
			            throw new Exception(String.Format("Error loading grammar file " + GrammarFileName + ": hr=0x{0:X}", rc));
			        }
				}
				
				if(manager.IsPhraseRecognized())
				{
					phraseTagRecognized.Value = manager.GetPhraseTagRecognized();
					manager.ClearPhraseRecognized();
					
					if(phraseDetectedEvent != null &&
						(expectedPhraseTag.Value == String.Empty || expectedPhraseTag.Value.Equals(phraseTagRecognized.Value,StringComparison.CurrentCultureIgnoreCase)))
					{
						Fsm.Event(phraseDetectedEvent);
					}
				}
				else
				{
					//phraseTagRecognized.Value = String.Empty;
				}
			}
		}
	}
}