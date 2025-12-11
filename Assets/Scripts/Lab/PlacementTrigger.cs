using UnityEngine;
using System.Collections;

public class PlacementTrigger : MonoBehaviour
{
    [Tooltip("Optional: Name oder Tag des Objekts, das hierhin gestellt werden soll.")]
    public string expectedTag = "SampleVessel"; 

   private void OnTriggerEnter(Collider other)
{
    Debug.Log("trigger enter1");
    var exp = LabManager.Instance?.experiments[0];
    if (exp == null) return;
    var step = exp.GetCurrentStep();
    if (step == null) return;
    Debug.Log("trigger enter");
     if (step.stepType == StepType.Place)
     {

         Debug.Log("type place object");
            bool correctObject = other.CompareTag(step.targetTag);
            bool correctLocation = (step.runtimeTargetLocation == transform);
            bool correctLocationName = step.targetLocationName == gameObject.name;
            Debug.Log(correctLocationName);
            Debug.Log(step.targetLocationName);
            Debug.Log(gameObject.name);
            if (correctObject && correctLocationName)
            {
                Debug.Log($"✅ Richtiger Gegenstand {other.name} an richtiger Stelle {transform.name} platziert!");

                LabManager.Instance.OnTaskCompleted();
            }

        }
           
    }


private IEnumerator WaitThenComplete(LabExperiment exp, float delay)
{
    yield return new WaitForSeconds(delay);
    exp.CompleteCurrentStep();
}


  
}
