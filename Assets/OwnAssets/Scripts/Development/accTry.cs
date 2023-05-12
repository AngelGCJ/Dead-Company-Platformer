using UnityEngine;

public class accTry : MonoBehaviour
{
    [SerializeField] public float counter = 0, seconds = 1;

    float fraction;
    float time;
    private void Start()
    {
        counter = 0;
    }

    private void Update()
    {
        /*
        if(transform.position.y < endPos - 0.1f)
        {
            counter += Time.deltaTime / rate;
            transform.position = new Vector3(transform.position.x, CalculateA(counter) * endPos, 0);
            Debug.Log(transform.position.y);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, endPos, 0);
            Debug.Log("SNAP");
        }*/

        

        if (counter < 10)
        {
            fraction = ExpIncrement(counter);
            counter += Time.deltaTime * 10/seconds;
        }
        else
        {
            fraction = 1;
        }
    }

    private float ExpIncrement(float x)
    {
        return -Mathf.Exp(-x) + 1;
    }




}
