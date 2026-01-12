using UnityEngine;

public class AttackDetection : MonoBehaviour
{
    [SerializeField] private PatrollPhysics patrollPhysics;
    private void OnTriggerStay2D(Collider2D collision)
    {
        patrollPhysics.inAttackRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        patrollPhysics.inAttackRange = false;
    }
}
