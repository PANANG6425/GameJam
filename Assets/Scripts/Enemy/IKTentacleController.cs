using UnityEngine;

public class IKTentacleController : MonoBehaviour
{
    // Drag the generated IK Target GameObject here
    public Transform ikTarget; 
    public float followSpeed = 5f;

    [Header("Random Targets")]
    public Transform[] randomTargets;
    public float targetChangeInterval = 2f;

    private Transform currentTarget;
    private float timer;

    void Start()
    {
        PickNewTarget();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PickNewTarget();
        }

        if (currentTarget != null)
        {
            Vector3 targetPos = currentTarget.position;
            targetPos.z = 0f; // Keep it on the 2D plane

            // Smoothly interpolate the IK target position
            ikTarget.position = Vector3.Lerp(ikTarget.position, targetPos, followSpeed * Time.deltaTime);
        }
    }

    void PickNewTarget()
    {
        timer = targetChangeInterval;
        if (randomTargets != null && randomTargets.Length > 0)
        {
            int randomIndex = Random.Range(0, randomTargets.Length);
            currentTarget = randomTargets[randomIndex];
        }
    }
}