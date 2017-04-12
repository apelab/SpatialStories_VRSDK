using UnityEngine;
using VR = UnityEngine.VR;
using System.Collections;

/// <summary>
/// Shows the Oculus plaform UI.
/// </summary>
public class OVRPlatformMenu : MonoBehaviour
{
	/// <summary>
	/// The key code.
	/// </summary>
	public KeyCode keyCode = KeyCode.Escape;

	public enum eHandler
	{
		ShowConfirmQuit,
	};

	public eHandler shortPressHandler = eHandler.ShowConfirmQuit;

	private float doubleTapDelay = 0.25f;
	private float shortPressDelay = 0.25f;
	private float longPressDelay = 0.75f;

	enum eBackButtonAction
	{
		NONE,
		DOUBLE_TAP,
		SHORT_PRESS
	};

	private int downCount = 0;
	private int upCount = 0;
	private float initialDownTime = -1.0f;

	eBackButtonAction ResetAndSendAction( eBackButtonAction action )
	{
		print( "ResetAndSendAction( " + action + " );" );
		downCount = 0;
		upCount = 0;
		initialDownTime = -1.0f;
		return action;
	}

	eBackButtonAction HandleBackButtonState() 
	{
		if ( Input.GetKeyDown( keyCode ) )
		{
			// just came down
			downCount++;
			if ( downCount == 1 )
			{
				initialDownTime = Time.realtimeSinceStartup;
			}
		}
		else if ( downCount > 0 )
		{
			if ( Input.GetKey( keyCode ) )
			{
				if ( downCount <= upCount )
				{
					// just went down
					downCount++;
				}

				float timeSinceFirstDown = Time.realtimeSinceStartup - initialDownTime;
				if ( timeSinceFirstDown > longPressDelay )
				{
					return ResetAndSendAction( eBackButtonAction.NONE );
				}
			}
			else
			{
				bool started = initialDownTime >= 0.0f;
				if ( started )
				{
					if ( upCount < downCount )
					{
						// just came up
						upCount++;
					}

					float timeSinceFirstDown = Time.realtimeSinceStartup - initialDownTime;
					if (timeSinceFirstDown < doubleTapDelay)
					{
						if (downCount == 2 && upCount == 2)
						{
							return ResetAndSendAction(eBackButtonAction.DOUBLE_TAP);
						}
					}
					else if (timeSinceFirstDown > shortPressDelay && timeSinceFirstDown < longPressDelay)
					{
						if (downCount == 1 && upCount == 1)
						{
							return ResetAndSendAction(eBackButtonAction.SHORT_PRESS);
						}
					}
					else if (timeSinceFirstDown > longPressDelay)
					{
						return ResetAndSendAction(eBackButtonAction.NONE);
					}
				}
			}
		}

		// down reset, but perform no action
		return eBackButtonAction.NONE;
	}

	/// <summary>
	/// Instantiate the cursor timer
	/// </summary>
	void Awake()
	{
		if (!OVRManager.isHmdPresent)
		{
			enabled = false;
			return;
		}
	}

	/// <summary>
	/// Reset when resuming
	/// </summary>
	void OnApplicationFocus( bool focusState )
	{
		//Input.ResetInputAxes();
		//ResetAndSendAction( eBackButtonAction.NONE );
	}

	/// <summary>
	/// Reset when resuming
	/// </summary>
	void OnApplicationPause( bool pauseStatus ) 
	{
		if ( !pauseStatus )
		{
			Input.ResetInputAxes();
		}
		//ResetAndSendAction( eBackButtonAction.NONE );
	}

	/// <summary>
	/// Show the confirm quit menu
	/// </summary>
	void ShowConfirmQuitMenu()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log("[PlatformUI-ConfirmQuit] Showing @ " + Time.time);
		OVRManager.PlatformUIConfirmQuit();
#endif
	}

	void DoHandler(eHandler handler)
	{
		if (handler == eHandler.ShowConfirmQuit)
			ShowConfirmQuitMenu ();
	}

	/// <summary>
	/// Tests for long-press and activates global platform menu when detected.
	/// as per the Unity integration doc, the back button responds to "mouse 1" button down/up/etc
	/// </summary>
	void Update()
	{
#if UNITY_ANDROID
		eBackButtonAction action = HandleBackButtonState();
		if (action == eBackButtonAction.SHORT_PRESS)
			DoHandler(shortPressHandler);
#endif
	}
}
