using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }

    public bool AttackTriggered { get; private set; }
    public bool Skill1Triggered { get; private set; }
    public bool Skill2Triggered { get; private set; }
    public bool Skill3Triggered { get; private set; }
    public bool Select1 { get; private set; }
    public bool Select2 { get; private set; }
    public bool Select3 { get; private set; }
    public bool Select4 { get; private set; }
    public bool Select5 { get; private set; }
    public bool JumpTriggered { get; private set; }

    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.PlayerControll.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
        inputActions.PlayerControll.Move.canceled += ctx => Move = Vector2.zero;

        inputActions.PlayerControll.Look.performed += ctx => Look = ctx.ReadValue<Vector2>();
        inputActions.PlayerControll.Look.canceled += ctx => Look = Vector2.zero;

        inputActions.PlayerControll.Attack.performed += _ => AttackTriggered = true;
        inputActions.PlayerControll.Skill1.performed += _ => Skill1Triggered = true;
        inputActions.PlayerControll.Skill2.performed += _ => Skill2Triggered = true;
        inputActions.PlayerControll.Skill_Ult.performed += _ => Skill3Triggered = true;

        inputActions.PlayerControll.Select1.performed += _ => Select1 = true;
        inputActions.PlayerControll.Select2.performed += _ => Select2 = true;
        inputActions.PlayerControll.Select3.performed += _ => Select3 = true;
        inputActions.PlayerControll.Select4.performed += _ => Select4 = true;
        inputActions.PlayerControll.Select5.performed += _ => Select5 = true;

        inputActions.PlayerControll.Jump.performed += _ => JumpTriggered = true;
    }

    private void LateUpdate()
    {
        // 한 프레임만 true로 유지
        AttackTriggered = false;
        Skill1Triggered = false;
        Skill2Triggered = false;
        Skill3Triggered = false;

        Select1 = false;
        Select2 = false;
        Select3 = false;
        Select4 = false;
        Select5 = false;

    }
    public void ConsumeJump()
    {
        JumpTriggered = false;
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}
