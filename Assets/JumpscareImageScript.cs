using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpscareImageScript : MonoBehaviour
{
    public GameObject JumpScareImg;     // Corrected the colon to semicolon
    public AudioSource audioSource;     // AudioSource reference for jumpscare sound

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the jumpscare image is hidden at the start
        JumpScareImg.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) // Check if the player triggered it
        {
            // Show the jumpscare image
            JumpScareImg.SetActive(true);

            // Play the jumpscare sound
            audioSource.Play();

            // Start the coroutine to hide the image after 2 seconds
            StartCoroutine(DisableImg());
        }
    }

    // Coroutine to disable the jumpscare image after 2 seconds
    IEnumerator DisableImg()
    {
        yield return new WaitForSeconds(2);
        JumpScareImg.SetActive(false);
    }
}
