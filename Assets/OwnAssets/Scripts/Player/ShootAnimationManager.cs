using System.Collections;
using UnityEngine;

public class ShootAnimationManager : MonoBehaviour
{
    [SerializeField] GameObject _leftArm, _rightArm, _shootRunRight, _shootRunLeft;
    [SerializeField] float _recoilDistance = 1f;
    bool _shooting;
    float _duration = 0.5f;
    public bool _facingRight;
    GameObject _leftShootingArm, _rightShootingArm;
    Vector3 _initialPosRight, _initialPosLeft;


    private void Start()
    {
        //_leftShootingArm = _leftShootingArm.GetComponent<Transform>();
        //_rightShootingArm = _rightShootingArm.GetComponent<Transform>();

    }

    private void Update()
    {
        if (_shooting)
        {
            if (_facingRight)
            {
                _rightArm.SetActive(false);
                _shootRunRight.SetActive(true);

                _leftArm.SetActive(true);
                _shootRunLeft.SetActive(false);
            }
            else if (!_facingRight)
            {
                _leftArm.SetActive(false);
                _shootRunLeft.SetActive(true);

                _rightArm.SetActive(true);
                _shootRunRight.SetActive(false);
            }
        }
        else
        {
            _rightArm.SetActive(true);
            _shootRunRight.SetActive(false);

            _leftArm.SetActive(true);
            _shootRunLeft.SetActive(false);
        }
    }

    public void Shoot(float time)
    {
        StopAllCoroutines();
        StartCoroutine(AnimShoot(time));
    }

    IEnumerator AnimShoot(float time)
    {
        _shooting = true;
        yield return new WaitForSeconds(time);
        _shooting = false;
    }
}
