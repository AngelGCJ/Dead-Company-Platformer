using System.Collections;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    SpriteRenderer sprite;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        while(sprite.color.a > 0)
        {
            sprite.color = new Color (sprite.color.a, sprite.color.g, sprite.color.b, sprite.color.a - 0.05f);
            yield return new WaitForSeconds(0.1f);
            //Debug.Log(sprite.color);
        }
        Destroy(gameObject);
    }
}
