using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Vespa : Enemy, IEnemy
{
    public NavMeshAgent agent;
    public List<Transform> wayPoints = new();
    public float velocity;

    private Transform actualPoint;

    public override void AttackingState()
    {
        base.AttackingState();
    }

    public override void FollowState()
    {
        base.FollowState();

    }

    private void SelectNextWayPoint(Transform actual = null)
    {
        if (actual != null)
        {
            int index = (wayPoints.IndexOf(actualPoint) + 1);

        }
    }

    public override void IdleState()
    {
        base.IdleState();
    }

    public override void DeadState()
    {
        base.DeadState();
    }

    public void ReceiveDamage(int damageDealed)
    {
    }

    public IEnumerator GetStuned(float timeStuned)
    {
        yield return null;
    }
}
