using UnityEngine;

public class CameraScript2 : MonoBehaviour
{
    public Transform player;
    public float fixedY = 0f;              // standaard hoogte van de camera
    public float yThreshold = 2f;          // hoeveel hoger speler mag gaan voor camera meebeweegt
    public float smoothSpeed = 0.125f;
    public float leftClampX = -8f;

    private float dynamicY;                // huidige doel-Y van camera

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

        dynamicY = fixedY;
        transform.position = new Vector3(leftClampX, fixedY, transform.position.z);
    }

    void LateUpdate()
    {
        if (player == null) return;

        // --- Y-CAMERA LOGICA ---
        // Als speler hoger is dan threshold → camera volgt omhoog
        if (player.position.y > fixedY + yThreshold)
        {
            dynamicY = Mathf.Lerp(dynamicY, player.position.y - yThreshold, smoothSpeed);
        }
        // Als speler terugzakt → camera keert geleidelijk terug naar vaste hoogte
        else if (dynamicY > fixedY)
        {
            dynamicY = Mathf.Lerp(dynamicY, fixedY, smoothSpeed);
        }

        // --- X-CAMERA LOGICA ---
        Vector3 targetPos = new Vector3(player.position.x, dynamicY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed);

        // Clamping aan de linkerzijde
        if (transform.position.x < leftClampX)
            transform.position = new Vector3(leftClampX, transform.position.y, transform.position.z);
    }
}
