using System.Diagnostics;
using UnityEngine;
using System.Collections;

public class TriggerWelcomeSound : MonoBehaviour
{
    private AudioSource audioSource;
    private bool hasPlayed = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


	private IEnumerator PlayWelcome(float delaySeconds)
	{
		yield return new WaitForSeconds(delaySeconds);
		audioSource.Play();
		hasPlayed = true;

	}

	private void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag("Player"))
        {

			StartCoroutine(PlayWelcome(3f));
		}
    }
}
