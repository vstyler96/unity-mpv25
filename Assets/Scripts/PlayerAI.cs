using UnityEngine;

public partial class Player
{
    [SerializeField] private float aiSpeed = 0.75f; // fraction of moveSpeed AI players use

    private Vector3 homeSpot; // where this player spawned; Portero and Defensa never leave it

    private Vector3 ThinkAI()
    {
        Vector3 ball = Match.Instance.Ball.transform.position;
        // Kick() already ignores this unless the ball is in reach and the cooldown is over.
        Kick(Match.Instance.EnemyGoal(team) - ball);

        Vector3 target = role == Role.Shooter ? ball : homeSpot;
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0f;
        return toTarget.magnitude < 0.3f ? Vector3.zero : toTarget.normalized * aiSpeed;
    }
}
