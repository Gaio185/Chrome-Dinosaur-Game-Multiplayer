using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedSprite : MonoBehaviour
{
    public Sprite[] sprites;

    private SpriteRenderer spriteRenderer;
    private int frame;

    //private void Awake()
    //{
        
    //}

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke(nameof(Animate), 0f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Animate()
    {
        if (GameManager.Instance.enabled)
        {
            frame++;

            if (frame >= sprites.Length)
            {
                frame = 0;
            }

            if (frame >= 0 && frame < sprites.Length)
            {
                spriteRenderer.sprite = sprites[frame];
            }

            Invoke(nameof(Animate), 1f / GameManager.Instance.gameSpeed);

        }
        
    }
}
