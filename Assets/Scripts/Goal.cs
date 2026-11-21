using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    private Team scoringTeam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) {
            Match.Instance.Scored(scoringTeam);
        }
    }
}
