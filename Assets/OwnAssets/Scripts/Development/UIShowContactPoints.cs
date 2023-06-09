using UnityEngine;

public class UIShowContactPoints : MonoBehaviour
{

    [SerializeField] public GameObject up, left, right, down;
    [SerializeField] public PlayerStateMachine playerState;

    private void Start()
    {
        up.SetActive(false);
        right.SetActive(false);
        left.SetActive(false);
        down.SetActive(false);
    }
    void Update()
    {
        if (playerState.IsGrounded) { down.SetActive(true); }
        else { down.SetActive(false); }
        //
        if (playerState.WallOnLeft) { left.SetActive(true); }
        else { left.SetActive(false); }
        //
        if (playerState.WallOnRight) { right.SetActive(true); }
        else { right.SetActive(false); }
        //
    
    }
}
