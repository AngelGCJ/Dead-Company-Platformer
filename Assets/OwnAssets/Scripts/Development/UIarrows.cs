using UnityEngine;

public class UIarrows : MonoBehaviour
{
    [SerializeField] public Animator arrowLeft, jump, arrowRight;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        { arrowLeft.SetBool("transition", true); } 
        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
        { arrowLeft.SetBool("transition", false); }
        //
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        { arrowRight.SetBool("transition", true); }
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
        { arrowRight.SetBool("transition", false); }
        //
        if (Input.GetKeyDown(KeyCode.Space))
        { jump.SetBool("transition", true); }
        if (Input.GetKeyUp(KeyCode.Space))
        { jump.SetBool("transition", false); }
    }
}
