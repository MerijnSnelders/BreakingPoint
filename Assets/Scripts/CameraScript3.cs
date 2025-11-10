using UnityEngine;

public class CameraScript3 : MonoBehaviour
{
    public Transform player;

    [Header("Y Follow Settings")]
    public float fixedY = 0f;           // default camera height
    public float yThreshold = 2f;       // how far player can go up before camera moves
    public float smoothSpeed = 0.125f;

    [Header("Clamp Settings")]
    public float leftClampX = -8f;

    [Header("Multi-Level Settings")]
    public float lowerZoneThreshold = -15f;   // when player goes below this → switch camera base
    public float lowerFixedY = -20f;          // camera base in lower area

    private float dynamicY;
    private float currentBaseY;

    public GameObject upperBackground;
    public GameObject lowerBackground;

    void Start()
    {
        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                player = obj.transform;
        }

        if (player == null)
        {
            Debug.LogError("❌ Player not found — add 'Player' tag!");
            return;
        }

        currentBaseY = fixedY;
        dynamicY = currentBaseY;
        transform.position = new Vector3(leftClampX, currentBaseY, transform.position.z);
        upperBackground.SetActive(true);
        lowerBackground.SetActive(false);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // --- 🔄 Switch camera zone automatically ---
        if (player.position.y < lowerZoneThreshold)
        {
            lowerBackground.SetActive(true);
            upperBackground.SetActive(false);
            currentBaseY = lowerFixedY;
        }
        else
        {
            if(!upperBackground.active)
                upperBackground.SetActive(true);
            currentBaseY = fixedY;
        }
            

        // --- 🧠 Smooth vertical follow logic ---
        if (player.position.y > currentBaseY + yThreshold)
        {
            dynamicY = Mathf.Lerp(dynamicY, player.position.y - yThreshold, smoothSpeed);
        }
        else if (dynamicY > currentBaseY)
        {
            dynamicY = Mathf.Lerp(dynamicY, currentBaseY, smoothSpeed);
        }

        // --- 📸 Smooth camera follow ---
        Vector3 targetPos = new Vector3(player.position.x, dynamicY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        // --- ⛔ Left clamp ---
        if (transform.position.x < leftClampX)
            transform.position = new Vector3(leftClampX, transform.position.y, transform.position.z);
    }
}

