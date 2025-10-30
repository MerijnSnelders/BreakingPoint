using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFallingSpikeScript : MonoBehaviour
{
    [Header("Spikes to control")]
    public GameObject[] spikes; // voeg hier je spikes toe (links → rechts)

    [Header("Settings")]
    public float fallDelay = 0.3f; // tijd tussen elke vallende spike
    public bool randomOrder = true; // of ze willekeurig vallen

    private bool hasStarted = false;
    private bool[] fallen; // houdt bij welke spikes al gevallen zijn

    private void Awake()
    {
        fallen = new bool[spikes.Length];

        // Zet alle spikes eerst op kinematic
        foreach (var spike in spikes)
        {
            if (spike != null)
            {
                Rigidbody2D rb = spike.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.gravityScale = 1f; // klaar om te vallen
                }
            }
        }
    }

    public void StartFalling()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            StartCoroutine(FallRoutine());
        }
    }

    private IEnumerator FallRoutine()
    {
        int groupSize = 5; // aantal spikes per groep

        for (int start = 0; start < spikes.Length; start += groupSize)
        {
            int end = Mathf.Min(start + groupSize, spikes.Length);
            List<int> groupIndices = new List<int>();

            // Voeg spikes van deze groep toe (bijv. 0–4, 5–9, ...)
            for (int i = start; i < end; i++)
                groupIndices.Add(i);

            // Kies willekeurig 2 of 3 spikes uit deze groep
            int spikesToDrop = Random.Range(2, 4);
            List<int> selectedIndices = new List<int>();

            for (int i = 0; i < spikesToDrop && groupIndices.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, groupIndices.Count);
                selectedIndices.Add(groupIndices[randomIndex]);
                groupIndices.RemoveAt(randomIndex);
            }

            // Laat de gekozen spikes één voor één vallen
            foreach (int i in selectedIndices)
            {
                if (i > (selectedIndices.Count / 2))
                    fallDelay = 0.4f;
                GameObject spike = spikes[i];
                if (spike != null)
                {
                    Rigidbody2D rb = spike.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.bodyType = RigidbodyType2D.Dynamic;
                        fallen[i] = true;
                    }
                }

                // korte pauze tussen spikes in dezelfde groep
                yield return new WaitForSeconds(fallDelay);
            }
        }
    }
}
