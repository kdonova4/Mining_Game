
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceLaunchController : MonoBehaviour
{

    Collider m_ResourceCollector;
    int m_ResourceCount;
    HashSet<GameObject> resourcesInCargoHold = new();
    public TextMeshProUGUI DisplayText;
    public Animator RocketAnimator;
    public PayoutManager PayoutManager;
    public CreditsManager CreditsManager;
    public bool m_IsLaunched { get; set; }

    int credits;
    void Awake()
    {
        m_ResourceCollector = GetComponent<Collider>();
        credits = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DisplayText.text = $"Cargo: {resourcesInCargoHold.Count}";
    }

    public void HandleLaunch()
    {
        if (m_IsLaunched) return;

        StartCoroutine(LaunchSequence());
    }

    private IEnumerator LaunchSequence()
    {
        m_IsLaunched = true;

        RocketAnimator.SetBool("Open", false);

        yield return new WaitForSeconds(3.5f);

        int payout = PayoutManager.CalculatePayout(resourcesInCargoHold);
        CreditsManager.AddCredits(payout);

        foreach(GameObject resource in resourcesInCargoHold)
        {
            Destroy(resource);
        }

        resourcesInCargoHold.Clear();

        RocketAnimator.SetBool("Open", true);

        m_IsLaunched = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsValid(other))
        {
            resourcesInCargoHold.Add(other.gameObject);
        }

        Resource resource = other.gameObject.GetComponent<Resource>();
        Ore ore = other.gameObject.GetComponent<Ore>();
        if (ore)
        {
            ore.IsDynamic = false;
        } else if (resource)
        {
            resource.isDynamic = false;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        resourcesInCargoHold.Remove(other.gameObject);
    }

    private bool IsValid(Collider other)
    {
        return other.attachedRigidbody != null &&
           (other.TryGetComponent<Resource>(out _) ||
            other.TryGetComponent<Ore>(out _));
    }
}
