using UnityEngine;
using UnityEngine.UI;

public class NeedsUI : MonoBehaviour
{
    [Header("References")]
    public NeedsManager needsManager;

    [Header("UI Elements")]
    public Image hungerFill;
    public Image energyFill;
    public Image cleanlinessFill;

    [Header("Lerp Settings")]
    public float lerpSpeed = 1f; // how fast bars smooth toward target

    // internal current and target fill values (0–1)
    private float targetHunger;
    private float targetEnergy;
    private float targetCleanliness;

    private float currentHunger;
    private float currentEnergy;
    private float currentCleanliness;

    void Start()
    {
        if (needsManager == null)
            needsManager = FindFirstObjectByType<NeedsManager>();

        // Initialize bars
        currentHunger = targetHunger = needsManager.Hunger / 100f;
        currentEnergy = targetEnergy = needsManager.Energy / 100f;
        currentCleanliness = targetCleanliness = needsManager.Cleanliness / 100f;
    }

    public void SetTargets(float hunger, float energy, float cleanliness)
    {
        targetHunger = Mathf.Clamp01(hunger / 100f);
        targetEnergy = Mathf.Clamp01(energy / 100f);
        targetCleanliness = Mathf.Clamp01(cleanliness / 100f);
    }

    void Update()
    {
        // Smoothly interpolate current fills toward target fills
        currentHunger = Mathf.Lerp(currentHunger, targetHunger, Time.deltaTime * lerpSpeed);
        currentEnergy = Mathf.Lerp(currentEnergy, targetEnergy, Time.deltaTime * lerpSpeed);
        currentCleanliness = Mathf.Lerp(currentCleanliness, targetCleanliness, Time.deltaTime * lerpSpeed);

        // Apply to UI
        hungerFill.fillAmount = currentHunger;
        energyFill.fillAmount = currentEnergy;
        cleanlinessFill.fillAmount = currentCleanliness;
    }
}
