using Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
    jumpBufferTime
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
