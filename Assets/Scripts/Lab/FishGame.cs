using UnityEngine;
using TMPro;

public class FishGame : MonoBehaviour
{
    [Header("Cards & Positions")]
    public GameObject[] nameCards;
    public Transform[] cardDefaultPos;
    public Transform[] cardOnImagePos;

    [Header("Effects & UI")]
    public PulseEffect[] effects;
    public TextMeshProUGUI scoreTextUI;

    [Header("Game Rules")]
    [Tooltip("correctMapping[cardIndex] = slotIndex where that card belongs")]
    public int[] correctMapping;

    private GameObject selectedCard;
    private int selectedCardNumber = -1;

    // cardAssignments[slot] = cardIndex, -1 if empty
    private int[] cardAssignments;

    private void Start()
    {
        StopAllEffects();
        cardAssignments = new int[cardOnImagePos.Length];
        for (int i = 0; i < cardAssignments.Length; i++)
            cardAssignments[i] = -1;
        UpdateScoreUI();
    }

    public void SelectNameCard(int cardNumber)
    {
        StopAllEffects();
        selectedCardNumber = cardNumber;
        selectedCard = nameCards[cardNumber];

        var effect = selectedCard.GetComponent<PulseEffect>();
        if (effect != null) effect.pulseAmount = 0.1f;
    }

    private void StopAllEffects()
    {
        foreach (var e in effects)
            if (e != null) e.pulseAmount = 0f;
    }

    public void TryPlaceCard(int imageNumber)
    {
        if (selectedCard == null) return;
        if (imageNumber < 0 || imageNumber >= cardOnImagePos.Length) return;

        // Clear old assignment
        ClearOldAssignment(selectedCardNumber);

        // If slot occupied → swap cards
        if (cardAssignments[imageNumber] != -1)
        {
            int otherCardIndex = cardAssignments[imageNumber];
            GameObject otherCard = nameCards[otherCardIndex];

            // Move the other card back to its default position
            otherCard.transform.position = cardDefaultPos[otherCardIndex].position;
            otherCard.transform.rotation = cardDefaultPos[otherCardIndex].rotation;

            cardAssignments[imageNumber] = -1;

            // Adjust score if that card was correct
            if (correctMapping[otherCardIndex] == imageNumber && GameManager2.Instance.fishGameResults[imageNumber])
            {
                GameManager2.Instance.fishGameResults[imageNumber] = false;
                GameManager2.Instance.fishGameScore--;
                UpdateScoreUI();
            }

            Debug.Log($"Swapped out card {otherCardIndex} from slot {imageNumber}");
        }

        // Place selected card
        selectedCard.transform.position = cardOnImagePos[imageNumber].position;
        selectedCard.transform.rotation = cardOnImagePos[imageNumber].rotation;

        var placedEffect = selectedCard.GetComponent<PulseEffect>();
        if (placedEffect != null) placedEffect.pulseAmount = 0f;

        cardAssignments[imageNumber] = selectedCardNumber;

        // ✅ Score logic using mapping
        if (correctMapping[selectedCardNumber] == imageNumber && !GameManager2.Instance.fishGameResults[imageNumber])
        {
            GameManager2.Instance.fishGameResults[imageNumber] = true;
            GameManager2.Instance.fishGameScore++;
            UpdateScoreUI();
        }

        Debug.Log($"Placed card {selectedCardNumber} on slot {imageNumber}");
    }

    public void MoveSelectedCardBackToDefault()
    {
        if (selectedCard == null) return;
        if (selectedCardNumber < 0 || selectedCardNumber >= cardDefaultPos.Length) return;

        selectedCard.transform.position = cardDefaultPos[selectedCardNumber].position;
        selectedCard.transform.rotation = cardDefaultPos[selectedCardNumber].rotation;

        var effect = selectedCard.GetComponent<PulseEffect>();
        if (effect != null) effect.pulseAmount = 0f;

        ClearOldAssignment(selectedCardNumber);
        Debug.Log($"Card {selectedCardNumber} moved back to default.");
    }

    private void ClearOldAssignment(int cardNumber)
    {
        for (int i = 0; i < cardAssignments.Length; i++)
        {
            if (cardAssignments[i] == cardNumber)
            {
                cardAssignments[i] = -1;

                if (correctMapping[cardNumber] == i && GameManager2.Instance.fishGameResults[i])
                {
                    GameManager2.Instance.fishGameResults[i] = false;
                    GameManager2.Instance.fishGameScore--;
                    UpdateScoreUI();
                }
            }
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreTextUI != null)
            scoreTextUI.text = $"{GameManager2.Instance.fishGameScore}/{cardOnImagePos.Length}";
    }

public void SaveGameState()
{
    GameManager2 gm = GameManager2.Instance;
    gm.savedCardAssignments = (int[])cardAssignments.Clone();
    Debug.Log("FishGame state saved.");
}

public void RestoreGameState()
{
    GameManager2 gm = GameManager2.Instance;
    if (gm.savedCardAssignments == null) return; // nothing saved yet

    cardAssignments = (int[])gm.savedCardAssignments.Clone();

    // Move cards based on assignments
    for (int slot = 0; slot < cardAssignments.Length; slot++)
    {
        int cardIndex = cardAssignments[slot];
        if (cardIndex != -1)
        {
            nameCards[cardIndex].transform.position = cardOnImagePos[slot].position;
            nameCards[cardIndex].transform.rotation = cardOnImagePos[slot].rotation;
        }
        else
        {
            // If not assigned, move back to default
            nameCards[slot].transform.position = cardDefaultPos[slot].position;
            nameCards[slot].transform.rotation = cardDefaultPos[slot].rotation;
        }
    }

    // Recalculate score
    GameManager2.Instance.fishGameScore = 0;
    for (int slot = 0; slot < cardAssignments.Length; slot++)
    {
        int cardIndex = cardAssignments[slot];
        if (cardIndex != -1 && correctMapping[cardIndex] == slot)
        {
            GameManager2.Instance.fishGameResults[slot] = true;
            GameManager2.Instance.fishGameScore++;
        }
        else
        {
            GameManager2.Instance.fishGameResults[slot] = false;
        }
    }

    UpdateScoreUI();
    Debug.Log("FishGame state restored.");
}

public void ResetGame()
{
    // Clear assignments
    for (int i = 0; i < cardAssignments.Length; i++)
    {
        cardAssignments[i] = -1;
        GameManager2.Instance.fishGameResults[i] = false;
    }

    // Reset score
    GameManager2.Instance.fishGameScore = 0;

    // Move all cards back to their default positions
    for (int i = 0; i < nameCards.Length; i++)
    {
        if (nameCards[i] == null || cardDefaultPos[i] == null) continue;

        nameCards[i].transform.position = cardDefaultPos[i].position;
        nameCards[i].transform.rotation = cardDefaultPos[i].rotation;

        var effect = nameCards[i].GetComponent<PulseEffect>();
        if (effect != null) effect.pulseAmount = 0f;
    }

    // Update UI
    UpdateScoreUI();

    // Clear selection
    selectedCard = null;
    selectedCardNumber = -1;

    Debug.Log("FishGame reset to initial state.");
}

}
