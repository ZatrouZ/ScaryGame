using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HideTextAfterTime : MonoBehaviour
{
    public float hideDelay = 10f;  // Time after which the text will disappear (in seconds)
    public float fadeDuration = 2f; // Time taken for text to fade out

    private Text uiText;  // Reference to the Text component
    private Color originalColor;  // Store the original text color

    void Start()
    {
        // Get the Text component on the same GameObject this script is attached to
        uiText = GetComponent<Text>();

        // Store the original color of the text
        originalColor = uiText.color;

        // Start the coroutine to hide the text after a delay
        StartCoroutine(HideAndFadeText());
    }

    IEnumerator HideAndFadeText()
    {
        // Wait for the specified hideDelay time before starting to fade
        yield return new WaitForSeconds(hideDelay);

        float elapsedTime = 0f;

        // Gradually fade out the text over the fadeDuration
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);  // Gradually reduce the alpha
            uiText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);  // Set the new color with updated alpha
            yield return null;  // Wait for the next frame
        }

        // After fading out, hide the text completely
        uiText.gameObject.SetActive(false);  // You can also use `uiText.enabled = false;` to just disable the text
    }
}
