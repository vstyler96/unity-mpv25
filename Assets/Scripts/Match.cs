using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Team {
    Player,
    Cpu
}

public class Match : MonoBehaviour
{
    public const float FieldLength = 60f;
    public const float FieldWidth = 30f;

    public static Match Instance { get; private set; }

    [SerializeField]
    private Player playerPrefab;

    [SerializeField]
    private Transform[] playerSpawns;

    [SerializeField]
    private Transform[] cpuSpawns;

    [SerializeField]
    private Color playerColor = new Color(0.2f, 0.4f, 1f);

    [SerializeField]
    private Color cpuColor = new Color(1f, 0.25f, 0.2f);

    [SerializeField]
    private Ball ball;

    [SerializeField]
    private Transform playerGoal;

    [SerializeField]
    private Transform cpuGoal;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Text goalText;

    [SerializeField]
    private CinemachineCamera followCamera;

    [SerializeField]
    private float matchSeconds = 180f;

    private readonly List<Player> players = new List<Player>();
    private readonly List<Transform> spawns = new List<Transform>();
    private int playerScore;
    private int cpuScore;
    private float timeLeft;

    public Ball Ball => ball;
    public bool IsPlaying { get; private set; }
    public IReadOnlyList<Player> Players => players;

    public Vector3 EnemyGoal(Team t) => Flat(t == Team.Player ? cpuGoal : playerGoal);

    private static Vector3 Flat(Transform t) => new Vector3(t.position.x, 0f, t.position.z);

    private void Awake()
    {
        Instance = this;

        Spawn(playerSpawns, Team.Player, playerColor);
        Spawn(cpuSpawns, Team.Cpu, cpuColor);

        followCamera.Follow = players[0].transform;
        followCamera.LookAt = players[0].transform;
        timeLeft = matchSeconds;
        IsPlaying = true;
    }

    private void Spawn(Transform[] points, Team team, Color color)
    {
        for (int i = 0; i < points.Length; i++) {
            Player f = Instantiate(playerPrefab, points[i].position, points[i].rotation);
            f.Init(team, (Role)Mathf.Min(i, (int)Role.Guard), team == Team.Player && i == 0, color);
            players.Add(f);
            spawns.Add(points[i]);
        }
    }

    private void Update()
    {
        if (IsPlaying) {
            timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
            if (timeLeft <= 0f) {
                IsPlaying = false;
            }

            // Ball left the pitch (over a wall, through the back of a goal, or fell): drop it back at the center.
            Vector3 b = ball.transform.position;
            if (Mathf.Abs(b.x) > FieldWidth / 2f + 1f || Mathf.Abs(b.z) > FieldLength / 2f + 3f || b.y < -1f) {
                ball.ResetPhysics(new Vector3(0f, 0.5f, 0f));
            }
        }

        if (timeLeft <= 0f) {
            scoreText.text = $"FULL TIME  Player {playerScore} - {cpuScore} CPU  (press any key)";
            bool key = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
            bool pad = Gamepad.current != null
                && (Gamepad.current.buttonSouth.wasPressedThisFrame || Gamepad.current.startButton.wasPressedThisFrame);
            if (key || pad) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }

        int secs = Mathf.CeilToInt(timeLeft);
        scoreText.text = $"Player {playerScore} - {cpuScore} CPU   {secs / 60}:{secs % 60:00}";
    }

    public void Scored(Team scoringTeam)
    {
        if (!IsPlaying) return;
        if (scoringTeam == Team.Player) playerScore++;
        if (scoringTeam == Team.Cpu) cpuScore++;

        IsPlaying = false;
        StartCoroutine(Kickoff(scoringTeam));
    }

    private IEnumerator Kickoff(Team scoringTeam)
    {
        goalText.color = scoringTeam == Team.Player ? playerColor : cpuColor;
        goalText.text = "¡Goool!";
        yield return new WaitForSeconds(1.5f);
        goalText.text = "";

        for (int i = 0; i < players.Count; i++) {
            players[i].ResetTo(spawns[i].position, spawns[i].rotation);
        }
        ball.ResetPhysics(new Vector3(0f, 0.5f, 0f));
        IsPlaying = true;
    }
}
