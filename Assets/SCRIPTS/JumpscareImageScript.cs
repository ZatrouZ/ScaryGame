using System.Collections;
using UnityEngine;

public class JumpscareImageScript : MonoBehaviour
{
    public GameObject jumpScareImg; // Use camel case for variable names
    public AudioSource audioSource; // AudioSource reference for jumpscare sound

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the jumpscare image is hidden at the start
        if (jumpScareImg != null)
        {
            jumpScareImg.SetActive(false);
        }
        else
        {
            Debug.LogWarning("JumpScareImg GameObject is not assigned in the inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player triggered it
        if (other.CompareTag("Player"))
        {
            // Show the jumpscare image
            ShowJumpscare();
        }
    }

    // Method to show the jumpscare image and play sound
    private void ShowJumpscare()
    {
        // Show the jumpscare image
        if (jumpScareImg != null)
        {
            jumpScareImg.SetActive(true);
        }

        // Play the jumpscare sound
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource is not assigned in the inspector.");
        }

        // Start the coroutine to hide the image after a delay
        StartCoroutine(DisableImg());
    }

    // Coroutine to disable the jumpscare image after 2 seconds
    private IEnumerator DisableImg()
    {
        yield return new WaitForSeconds(2);
        if (jumpScareImg != null)
        {
            jumpScareImg.SetActive(false);
        }
    }
}
