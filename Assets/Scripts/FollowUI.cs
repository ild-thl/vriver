using UnityEngine;

public class UIFollowUI : MonoBehaviour
{
    public RectTransform target;       // das UI-Element, dem gefolgt werden soll
    public RectTransform follower;     // das UI-Element, das folgen soll

    void Update()
    {
        if (target == null || follower == null) return;

        // Position übernehmen (relativ zum Parent Canvas)
        follower.anchoredPosition = target.anchoredPosition;

        // Rotation übernehmen
        follower.localRotation = target.localRotation;

        // Optional: auch die Größe übernehmen
        // follower.sizeDelta = target.sizeDelta;
    }
}
