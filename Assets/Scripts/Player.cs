using UnityEngine;

// Fixed for the whole match, by spawn order: 0 = Shooter, 1 = Goalkeeper, 2+ = Guard.
public enum Role { Shooter, Goalkeeper, Guard }

public partial class Player : MonoBehaviour
{
    public Team team;
    public bool isHuman;
    public Role role;

    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private float sprintMultiplier = 1.5f;   // speed boost while Sprint (Shift) is held

    [SerializeField]
    private float kickRange = 1.6f;

    [SerializeField]
    private float kickForce = 3f;

    [SerializeField]
    private float kickCooldown = 0.5f;

    private InputSystem_Actions controller;
    private Rigidbody rigidBody;
    private Vector3 moveDir;
    private float nextKickTime;

    private void Awake()
    {
        controller = new InputSystem_Actions();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnDisable()
    {
        controller.Player.Disable();
    }

    private void OnDestroy()
    {
        controller.Dispose();
    }

    public void Init(Team team, Role role, bool isHuman, Color color)
    {
        this.team = team;
        this.role = role;
        this.isHuman = isHuman;

        homeSpot = transform.position;

        GetComponentInChildren<Renderer>().material.color = color;

        if (isHuman) {
            controller.Player.Enable();
        }
    }

    private void Update()
    {
        if (Match.Instance == null || !Match.Instance.IsPlaying) {
            moveDir = Vector3.zero;
            return;
        }

        if (isHuman) {
            Vector2 input = controller.Player.Move.ReadValue<Vector2>();
            moveDir = new Vector3(input.x, 0f, input.y);

            if (controller.Player.Sprint.IsPressed()) {
                moveDir *= sprintMultiplier;
            }

            if (controller.Player.Attack.WasPressedThisFrame()) {
                Kick(transform.forward);
            }

            return;
        }

        moveDir = ThinkAI();
    }

    private void FixedUpdate()
    {
        Vector3 v = moveDir * moveSpeed;
        rigidBody.linearVelocity = new Vector3(v.x, rigidBody.linearVelocity.y, v.z);

        if (moveDir.sqrMagnitude > 0.001f) {
            rigidBody.MoveRotation(Quaternion.LookRotation(new Vector3(moveDir.x, 0f, moveDir.z)));
        }
    }

    public void Kick(Vector3 dir)
    {
        if (Time.time < nextKickTime) {
            return;
        }
        Ball ball = Match.Instance.Ball;
        if (Vector3.Distance(transform.position, ball.transform.position) > kickRange) {
            return;
        }

        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) {
            return;
        }
        nextKickTime = Time.time + kickCooldown;
        Vector3 force = (dir.normalized + Vector3.up * 0.15f) * kickForce;
        ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
    }

    public void ResetTo(Vector3 pos, Quaternion rot)
    {
        rigidBody.position = pos;
        rigidBody.rotation = rot;
        transform.SetPositionAndRotation(pos, rot);
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        moveDir = Vector3.zero;
    }
}
