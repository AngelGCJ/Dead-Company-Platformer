using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Variables
{
    walkSpeed,
    runSpeed,
    maxJumpHeight,
    maxJumpTime,
    fallSpeedMultiplier,
    maxFallSpeedClamp,
    coyoteTime,
    jumpBufferTime,
    AccGround,
    DeAccGround,
    DeAccGroundTurn,
    AccAir,
    DeAccAir,
    DeAccAirTurn,
    FallEarly,
    WallSlideTime,
    WallSlideSpeed
}

public class ChangeVariables : MonoBehaviour
{
    //for buttons

    [SerializeField] public PlayerStateMachine context;
    public void SendWalkSpeed()
    {
        EventManager.Dispatch(Variables.walkSpeed, context.WalkSpeed);
    }
    public void SendRunSpeed()
    {
        EventManager.Dispatch(Variables.runSpeed, context.RunSpeed);
    }
    public void SendMaxJumpHeight()
    {
        EventManager.Dispatch(Variables.maxJumpHeight, context.MaxJumpHeight);
    }
    public void SendMaxJumpTime()
    {
        EventManager.Dispatch(Variables.maxJumpTime, context.MaxJumpTime);
    }
    public void SendFallSpeedMultiplier()
    {
        EventManager.Dispatch(Variables.fallSpeedMultiplier, context.FallMultiplier);
    }
    public void SendMaxFallSpeedClamp()
    {
        EventManager.Dispatch(Variables.maxFallSpeedClamp, context.FallSpeedClamp);
    }
    public void SendCoyoteTime()
    {
        EventManager.Dispatch(Variables.coyoteTime, context.CoyoteTime);
    }
    public void SendJumpBufferTime()
    {
        EventManager.Dispatch(Variables.jumpBufferTime, context.JumpBufferTime);
    }
    //
    public void SendAccGround()
    {
        EventManager.Dispatch(Variables.AccGround, context.AccGround);
    }
    public void SendDeAccGround()
    {
        EventManager.Dispatch(Variables.DeAccGround, context.DeAccGround);
    }
    public void SendDeAccGroundTurn()
    {
        EventManager.Dispatch(Variables.DeAccGroundTurn, context.DeAccGroundTurning);
    }
    public void SendAccAir()
    {
        EventManager.Dispatch(Variables.AccAir, context.AccAir);
    }
    public void SendDeAccAir()
    {
        EventManager.Dispatch(Variables.DeAccAir, context.DeAccAir);
    }
    public void SendDeAccAirTurn()
    {
        EventManager.Dispatch(Variables.DeAccAirTurn, context.DeAccAirTurning);
    }
    public void SendFallEarly()
    {
        EventManager.Dispatch(Variables.FallEarly, context.JumpToFallEarly);
    }
    //
    public void SendWallSlideTime()
    {
        EventManager.Dispatch(Variables.WallSlideTime, context.TimeToWallSlide);
    }
    public void SendWallSlideSpeed()
    {
        EventManager.Dispatch(Variables.WallSlideSpeed, context.WallSlideSpeed);
    }



    //changer:

    [SerializeField] public GameObject panel;
    [SerializeField] public Text variableName, oldValue;
    [SerializeField] public TMP_InputField inputField;

    public Variables currentVariable;

    public float myNewValue;

    private void Start()
    {
        panel.SetActive(false);
        EventManager.AddListener<float>(Variables.walkSpeed, PrepareChangeWalkSpeed);
        EventManager.AddListener<float>(Variables.runSpeed, PrepareChangeRunSpeed);
        EventManager.AddListener<float>(Variables.maxJumpHeight, PrepareChangeMaxJumpHeight);
        EventManager.AddListener<float>(Variables.maxJumpTime, PrepareChangeMaxJumpTime);
        EventManager.AddListener<float>(Variables.fallSpeedMultiplier, PrepareChangeFallSpeedMultiplier);
        EventManager.AddListener<float>(Variables.maxFallSpeedClamp, PrepareChangeMaxFallSpeedClamp);
        EventManager.AddListener<float>(Variables.coyoteTime, PrepareChangeCoyoteTime);
        EventManager.AddListener<float>(Variables.jumpBufferTime, PrepareChangeJumpBufferTime);
        //
        EventManager.AddListener<float>(Variables.AccGround, PrepareChangeAccGround);
        EventManager.AddListener<float>(Variables.DeAccGround, PrepareChangeDeAccGround);
        EventManager.AddListener<float>(Variables.DeAccGroundTurn, PrepareChangeDeAccGroundTurn);
        EventManager.AddListener<float>(Variables.AccAir, PrepareChangeAccAir);
        EventManager.AddListener<float>(Variables.DeAccAir, PrepareChangeDeAccAir);
        EventManager.AddListener<float>(Variables.DeAccAirTurn, PrepareChangeDeAccAirTurn);
        EventManager.AddListener<float>(Variables.FallEarly, PrepareFallEarly);
        //
        EventManager.AddListener<float>(Variables.WallSlideTime, PrepareWallSlideTime);
        EventManager.AddListener<float>(Variables.WallSlideSpeed, PrepareWallSlideSpeed);
    }

    private void OnDestroy()
    {
        EventManager.RemoveListener<float>(Variables.walkSpeed, PrepareChangeWalkSpeed);
        EventManager.RemoveListener<float>(Variables.runSpeed, PrepareChangeRunSpeed);
        EventManager.RemoveListener<float>(Variables.maxJumpHeight, PrepareChangeMaxJumpHeight);
        EventManager.RemoveListener<float>(Variables.maxJumpTime, PrepareChangeMaxJumpTime);
        EventManager.RemoveListener<float>(Variables.fallSpeedMultiplier, PrepareChangeFallSpeedMultiplier);
        EventManager.RemoveListener<float>(Variables.maxFallSpeedClamp, PrepareChangeMaxFallSpeedClamp);
        EventManager.RemoveListener<float>(Variables.coyoteTime, PrepareChangeCoyoteTime);
        EventManager.RemoveListener<float>(Variables.jumpBufferTime, PrepareChangeJumpBufferTime);
        //
        EventManager.RemoveListener<float>(Variables.AccGround, PrepareChangeAccGround);
        EventManager.RemoveListener<float>(Variables.DeAccGround, PrepareChangeDeAccGround);
        EventManager.RemoveListener<float>(Variables.DeAccGroundTurn, PrepareChangeDeAccGroundTurn);
        EventManager.RemoveListener<float>(Variables.AccAir, PrepareChangeAccAir);
        EventManager.RemoveListener<float>(Variables.DeAccAir, PrepareChangeDeAccAir);
        EventManager.RemoveListener<float>(Variables.DeAccAirTurn, PrepareChangeDeAccAirTurn);
        EventManager.RemoveListener<float>(Variables.FallEarly, PrepareFallEarly);
        //
        EventManager.RemoveListener<float>(Variables.WallSlideTime, PrepareWallSlideTime);
        EventManager.RemoveListener<float>(Variables.WallSlideSpeed, PrepareWallSlideSpeed);
    }

    //EVERY FUNCTION:
    public void PrepareChangeWalkSpeed(float value)
    {
        panel.SetActive(true);
        variableName.text = "Walk Speed";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.walkSpeed;
    }
    public void PrepareChangeRunSpeed(float value)
    {
        panel.SetActive(true);
        variableName.text = "Run Speed";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.runSpeed;
    }
    public void PrepareChangeMaxJumpHeight(float value)
    {
        panel.SetActive(true);
        variableName.text = "Max Jump Height";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.maxJumpHeight;
    }
    public void PrepareChangeMaxJumpTime(float value)
    {
        panel.SetActive(true);
        variableName.text = "Max Jump Time";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.maxJumpTime;
    }
    public void PrepareChangeFallSpeedMultiplier(float value)
    {
        panel.SetActive(true);
        variableName.text = "Fall Speed Multiplier";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.fallSpeedMultiplier;
    }
    public void PrepareChangeMaxFallSpeedClamp(float value)
    {
        panel.SetActive(true);
        variableName.text = "Max Fall Speed Clamp";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.maxFallSpeedClamp;
    }
    public void PrepareChangeCoyoteTime(float value)
    {
        panel.SetActive(true);
        variableName.text = "Coyote Time";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.coyoteTime;
    }
    public void PrepareChangeJumpBufferTime(float value)
    {
        panel.SetActive(true);
        variableName.text = "Jump Buffer Time";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.jumpBufferTime;
    }
    //
    public void PrepareChangeAccGround(float value)
    {
        panel.SetActive(true);
        variableName.text = "Acceleration while on ground";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.AccGround;
    }
    public void PrepareChangeDeAccGround(float value)
    {
        panel.SetActive(true);
        variableName.text = "deacceleration in ground";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.DeAccGround;
    }
    public void PrepareChangeDeAccGroundTurn(float value)
    {
        panel.SetActive(true);
        variableName.text = "deacc. turning in ground";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.DeAccGroundTurn;
    }
    public void PrepareChangeAccAir(float value)
    {
        panel.SetActive(true);
        variableName.text = "acceleration in air (if jumped)";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.AccAir;
    }
    public void PrepareChangeDeAccAir(float value)
    {
        panel.SetActive(true);
        variableName.text = "deacceleration in air (after jump)";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.DeAccAir;
    }
    public void PrepareChangeDeAccAirTurn(float value)
    {
        panel.SetActive(true);
        variableName.text = "deacc. trying to turn in air";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.DeAccAirTurn;
    }
    public void PrepareFallEarly(float value)
    {
        panel.SetActive(true);
        variableName.text = "fall a bit earlier";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.FallEarly;
    }
    //
    public void PrepareWallSlideTime(float value)
    {
        panel.SetActive(true);
        variableName.text = "wall slide time";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.WallSlideTime;
    }
    public void PrepareWallSlideSpeed(float value)
    {
        panel.SetActive(true);
        variableName.text = "wall slide speed";
        oldValue.text = "Old value: " + value.ToString();
        currentVariable = Variables.WallSlideSpeed;
    }

    public void ChangeButton()
    {
        if (currentVariable == Variables.walkSpeed)
        {
            context.WalkSpeed = myNewValue;
        }
        else if (currentVariable == Variables.runSpeed)
        {
            context.RunSpeed = myNewValue;
        }
        else if (currentVariable == Variables.maxJumpHeight)
        {
            context.MaxJumpHeight = myNewValue;
            context.setupJumpVariables();
        }
        else if (currentVariable == Variables.maxJumpTime)
        {
            context.MaxJumpTime = myNewValue;
            context.setupJumpVariables();
        }
        else if (currentVariable == Variables.fallSpeedMultiplier)
        {
            context.FallMultiplier = myNewValue;
        }
        else if (currentVariable == Variables.maxFallSpeedClamp)
        {
            context.FallSpeedClamp = myNewValue;
        }
        else if (currentVariable == Variables.coyoteTime)
        {
            context.CoyoteTime = myNewValue;
        }
        else if (currentVariable == Variables.jumpBufferTime)
        {
            context.JumpBufferTime = myNewValue;
        }
        //
        else if (currentVariable == Variables.AccGround)
        {
            context.AccGround = myNewValue;
        }
        else if (currentVariable == Variables.DeAccGround)
        {
            context.DeAccGround = myNewValue;
        }
        else if (currentVariable == Variables.DeAccGroundTurn)
        {
            context.DeAccGroundTurning= myNewValue;
        }
        else if (currentVariable == Variables.AccAir)
        {
            context.AccAir = myNewValue;
        }
        else if (currentVariable == Variables.DeAccAir)
        {
            context.DeAccAir = myNewValue;
        }
        else if (currentVariable == Variables.DeAccAirTurn)
        {
            context.DeAccAirTurning = myNewValue;
        }
        else if (currentVariable == Variables.FallEarly)
        {
            context.JumpToFallEarly = myNewValue;
        }
        else if (currentVariable == Variables.WallSlideTime)
        {
            context.TimeToWallSlide = myNewValue;
        }
        else if (currentVariable == Variables.WallSlideSpeed)
        {
            context.WallSlideSpeed = myNewValue;
        }
        else
        {
            Debug.Log("ERROR");
        }
    }

    //COMMON:
    public void InputField()
    {
        myNewValue = float.Parse(inputField.text);
        Debug.Log(myNewValue);
    }
    public void CancelButton()
    {
        panel.SetActive(false);
    }

}
