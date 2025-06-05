using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InteractiveAudioGuide : MonoBehaviour
{
	// Audio sources
	public AudioSource backgroundMusicSource;
	public AudioSource voiceoverSource;

	// Highlights
	public GameObject LCgripHighlight;
	public GameObject LCtriggerHighlight;
	public GameObject LCjoystickHighlight;
	public GameObject RCgripHighlight;
	public GameObject RCtriggerHighlight;
	public GameObject RCjoystickHighlight;

	// Audio clips
	public AudioClip AQ1;
	public AudioClip AQ2;
	public AudioClip AQ3;
	public AudioClip AQ4;
	public AudioClip AQ5;

	// XR Input Actions
	public InputActionReference leftJoystickAction;
	public InputActionReference rightJoystickAction;
	public InputActionReference leftTriggerAction;
	public InputActionReference leftGripAction;
	public InputActionReference rightTriggerAction;
	public InputActionReference rightGripAction;

	private void Start()
	{
		// Disable all highlights at start
		LCgripHighlight.SetActive(false);
		LCtriggerHighlight.SetActive(false);
		LCjoystickHighlight.SetActive(false);
		RCgripHighlight.SetActive(false);
		RCtriggerHighlight.SetActive(false);
		RCjoystickHighlight.SetActive(false);

		// Start background music
		if (backgroundMusicSource != null && backgroundMusicSource.clip != null)
		{
			backgroundMusicSource.loop = true;
			backgroundMusicSource.Play();
		}

		// Check if the audio guide has already been played for this scene
		string sceneKey = "AudioGuidePlayed_" + SceneManager.GetActiveScene().name;
		if (!PlayerPrefs.HasKey(sceneKey))
		{
			PlayerPrefs.SetInt(sceneKey, 1);
			PlayerPrefs.Save();

			StartCoroutine(InteractiveSequence());
		}
	}

	private IEnumerator InteractiveSequence()
	{
		yield return new WaitForSeconds(5f);

		// Play AQ1
		yield return StartCoroutine(PlayAudioClip(AQ1));
		yield return new WaitForSeconds(2f);

		// AQ2 - Wait for left joystick move
		yield return StartCoroutine(PlayAudioUntilUserAction(AQ2, LCjoystickHighlight, () => HasJoystickMoved(true)));

		// AQ3 - Wait for right joystick move
		yield return StartCoroutine(PlayAudioUntilUserAction(AQ3, RCjoystickHighlight, () => HasJoystickMoved(false)));

		yield return new WaitForSeconds(5f);

		// AQ4 - Wait for either grip press (highlights still show both grips)
		yield return StartCoroutine(PlayAudioUntilUserAction(AQ4, LCgripHighlight, RCgripHighlight, () => EitherGripPressed()));

		// AQ5 - Wait for either trigger press (highlights still show both triggers)
		yield return StartCoroutine(PlayAudioUntilUserAction(AQ5, LCtriggerHighlight, RCtriggerHighlight, () => EitherTriggerPressed()));
	}

	private bool EitherTriggerPressed()
	{
		float leftTrigger = leftTriggerAction.action.ReadValue<float>();
		float rightTrigger = rightTriggerAction.action.ReadValue<float>();
		return leftTrigger > 0.1f || rightTrigger > 0.1f;
	}

	private bool EitherGripPressed()
	{
		float leftGrip = leftGripAction.action.ReadValue<float>();
		float rightGrip = rightGripAction.action.ReadValue<float>();
		return leftGrip > 0.1f || rightGrip > 0.1f;
	}

	private IEnumerator PlayAudioClip(AudioClip clip)
	{
		voiceoverSource.clip = clip;
		voiceoverSource.Play();
		yield return new WaitForSeconds(clip.length);
	}

	private IEnumerator PlayAudioUntilUserAction(AudioClip clip, GameObject highlight, System.Func<bool> interactionCheck)
	{
		return PlayAudioUntilUserAction(clip, highlight, null, interactionCheck);
	}

	private IEnumerator PlayAudioUntilUserAction(AudioClip clip, GameObject highlight1, GameObject highlight2, System.Func<bool> interactionCheck)
	{
		bool interacted = false;
		bool isFirstPlayback = true;

		while (!interacted)
		{
			// Play audio
			voiceoverSource.clip = clip;
			voiceoverSource.Play();

			// Enable highlights
			if (highlight1) highlight1.SetActive(true);
			if (highlight2) highlight2.SetActive(true);

			float elapsedTime = 0f;

			while (voiceoverSource.isPlaying)
			{
				if (interactionCheck())
				{
					interacted = true;

					if (!isFirstPlayback)
					{
						voiceoverSource.Stop();

						if (highlight1) highlight1.SetActive(false);
						if (highlight2) highlight2.SetActive(false);

						yield break;
					}
				}

				elapsedTime += Time.deltaTime;
				yield return null;
			}

			// Disable highlights after audio
			if (highlight1) highlight1.SetActive(false);
			if (highlight2) highlight2.SetActive(false);

			if (interacted)
			{
				yield break;
			}
			else
			{
				isFirstPlayback = false;
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private bool HasJoystickMoved(bool isLeft)
	{
		Vector2 joystickInput = isLeft
			? leftJoystickAction.action.ReadValue<Vector2>()
			: rightJoystickAction.action.ReadValue<Vector2>();

		return joystickInput.magnitude > 0.1f;
	}

	private bool BothTriggersPressed()
	{
		float leftTrigger = leftTriggerAction.action.ReadValue<float>();
		float rightTrigger = rightTriggerAction.action.ReadValue<float>();
		return leftTrigger > 0.1f && rightTrigger > 0.1f;
	}

	private bool BothGripsPressed()
	{
		float leftGrip = leftGripAction.action.ReadValue<float>();
		float rightGrip = rightGripAction.action.ReadValue<float>();
		return leftGrip > 0.1f && rightGrip > 0.1f;
	}
}
