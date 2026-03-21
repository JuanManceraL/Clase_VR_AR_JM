using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    public MoveTarget moveTarget;
    public int targetId;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            moveTarget.PutAnimation(targetId);
        }
    }
}
