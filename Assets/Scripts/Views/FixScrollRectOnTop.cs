using UnityEngine;
using UnityEngine.UI;
using System.Collections; // Required for IEnumerator

[RequireComponent(typeof(ScrollRect))]
public class FixScrollRectOnTop : MonoBehaviour
{
    private ScrollRect scrollRect;

    void Awake()
    {
        // Get the ScrollRect component attached to this GameObject
        scrollRect = GetComponent<ScrollRect>();
    }

    void OnEnable()
    {
        // Start the coroutine to reset the position after layout has settled
        StartCoroutine(SetScrollPositionAtTop());
    }

    IEnumerator SetScrollPositionAtTop()
    {
        yield return new WaitForEndOfFrame();

        // Set the vertical normalized position to 1 (which means the top)
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1.0f;
        }

        // Optional: If it *still* doesn't work reliably,
        // wait one more frame just in case. Usually not needed.
        // yield return null;
        // if (scrollRect != null)
        // {
        //     scrollRect.verticalNormalizedPosition = 1.0f;
        // }
    }
}