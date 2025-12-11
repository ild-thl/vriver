using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScaleDevice : LabDevice
{
    [Header("Scale Settings")]
    public float currentWeight = 0f;
    public bool isZeroed = false;

    // Objects currently on the scale
    private readonly List<LabInteractable> itemsOnScale = new();

    // Offset when the scale is zeroed
    private float zeroOffset = 0f;

    [Header("UI")]
    public TextMeshProUGUI scaleUI;
    public TextMeshProUGUI scaleUI2;

    [Header("Debug")]
    public bool showDebug = true;

    private void Update()
    {
        if (itemsOnScale.Count > 0)
            UpdateScale();
    }

    public override void Interact()
    {
        if (!isZeroed)
        {
            ZeroScale();
        }
        else
        {
            UpdateScale();
            Debug.Log($"Aktuelles Gewicht: {currentWeight:F1} ml");
        }
    }

    public void ZeroScale()
    {
        zeroOffset = currentWeight;
        isZeroed = true;
        Debug.Log("Waage wurde genullt!");
        CheckStep();
    }

    public void AddItem(GameObject item)
    {
        Debug.Log("scalescale");
        var interactable = item.GetComponent<LabInteractable>();
        if (interactable == null) return;

        if (!itemsOnScale.Contains(interactable))
        {
            itemsOnScale.Add(interactable);
            if (showDebug)
                Debug.Log($"⚖️ {interactable.name} wurde auf die Waage gelegt.");
            
            UpdateScale();
        }
    }

    public void RemoveItem(GameObject item)
    {
        var interactable = item.GetComponent<LabInteractable>();
        if (interactable == null) return;

        if (itemsOnScale.Contains(interactable))
        {
            itemsOnScale.Remove(interactable);
            if (showDebug)
                Debug.Log($"⚖️ {interactable.name} wurde von der Waage entfernt.");
            
            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        float total = 0f;

        // Sum up dynamic weights of all items on scale
        foreach (var obj in itemsOnScale)
        {
            if (obj == null) continue;

            // Allow polymorphic behavior: PourableContainer, Pipette, etc.
            obj.UpdateWeight();
            total += obj.GetCurrentWeight();
        }

        currentWeight = Mathf.Max(0f, total - zeroOffset);
        UpdateScaleUI();

        if (showDebug)
        {
            Debug.Log($"⚖️ Neue Waageanzeige: {currentWeight:0.00} ml (Offset: {zeroOffset:0.00})");
        }
    }

    private void UpdateScaleUI()
    {
        if (scaleUI != null)
            scaleUI.text = $"{currentWeight:0.00} ml";
        
        if (scaleUI2 != null)
            scaleUI2.text = $"{currentWeight:0.00} ml";
    }

    private void CheckStep()
    {
        var exp = LabManager.Instance?.experiments[0];
        if (exp == null) return;
        var step = exp.GetCurrentStep();
        if (step == null) return;
        
        if (step.targetTag == "Scale")
        {

                Debug.Log($"✅ Richtiger Button");

                LabManager.Instance.OnTaskCompleted();
        }
        
    }
}
