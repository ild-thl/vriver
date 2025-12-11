using UnityEngine;

public class InteractionCollision : MonoBehaviour
{
    public GameObject interactionTipUI; // UI Element to show interaction hint
    public GameObject interactionTipUI_Mobile; // UI Element to show interaction hint
    public ParticleSystem interactionParticles; // Particle system to play on enter
    
    public ClickableObject co;

    private void Start()
    {
        // Ensure UI and particles are initially hidden
        if (interactionTipUI != null) interactionTipUI.SetActive(false);
        if (interactionParticles != null) interactionParticles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only player triggers it
        {
            
            GameManager2.Instance.isInInteractionCollider = true;
            GameManager2.Instance.currentCollision = this;
            

            if (GameManager2.Instance.isMobile)
            {
                ShowInteractionUI_Mobile();
            }
            else
            {
                ShowInteractionUI();
                
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure only player triggers it
        {
            HideInteractionUI();
            
            GameManager2.Instance.isInInteractionCollider = false;
            GameManager2.Instance.currentCollision = null;
        }
    }

    public void ShowInteractionUI()
    {
        
        UIManager uiM = UIManager.Instance;
        if (uiM != null)
        {
            interactionTipUI = uiM.interactionTipUI;
        }
            
        if (interactionTipUI != null) interactionTipUI.SetActive(true);
        //if (interactionTipUI_Mobile != null) interactionTipUI_Mobile.SetActive(false);
        //if (interactionParticles != null && !interactionParticles.isPlaying) interactionParticles.Play();
    }

    public void ShowInteractionUI_Mobile()
    {
        UIManager uiM = UIManager.Instance;
        if (uiM != null)
        {
            interactionTipUI = uiM.interactionTipUI;
        }
        if (interactionTipUI != null) interactionTipUI.SetActive(false);
        //if (interactionTipUI_Mobile != null) interactionTipUI_Mobile.SetActive(true);
        //if (interactionParticles != null && !interactionParticles.isPlaying) interactionParticles.Play();
    }

    public void HideInteractionUI()
    {
        UIManager uiM = UIManager.Instance;
        if (uiM != null)
        {
            interactionTipUI = uiM.interactionTipUI;
        }
        if (interactionTipUI != null) interactionTipUI.SetActive(false);
        //if (interactionTipUI_Mobile != null) interactionTipUI_Mobile.SetActive(false);
        //if (interactionParticles != null && interactionParticles.isPlaying) interactionParticles.Stop();
    }
}
