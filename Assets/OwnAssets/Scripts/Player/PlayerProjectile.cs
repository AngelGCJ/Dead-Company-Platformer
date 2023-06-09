using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField] float _lifespam = 2, _speed = 20;
    [HideInInspector] public float _direction = 1;
    float _timer;

    private void Start()
    {
        _timer = 0;
        transform.parent = null;
    }
    void Update()
    {
        _timer += Time.deltaTime;

        if(_timer >= _lifespam)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position += Vector3.right * Time.deltaTime * _speed * _direction;
        }
    }
}
