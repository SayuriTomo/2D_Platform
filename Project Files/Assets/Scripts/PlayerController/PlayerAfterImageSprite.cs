using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImageSprite : MonoBehaviour
{
    [SerializeField]
    private float ActiveTime = 0.1f;
    private float TimeActivated;
    private float Alpha;
    [SerializeField]
    private float AlphaSet = 0.8f;
    private float AlphaMultiplier = 0.85f;

    private Transform Player;

    private SpriteRenderer SR;
    private SpriteRenderer PlayerSR;

    private Color Color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Alpha *= AlphaMultiplier;
        Color = new Color(1f,1f, 1f, Alpha);
        SR.color = Color;

        if (Time.time >= (TimeActivated + ActiveTime))
        {
            PlayerAfterImagePool.Instance.AddToPool(gameObject);
        }
    }

    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerSR = Player.GetComponent<SpriteRenderer>();

        Alpha = AlphaSet;
        SR.sprite = PlayerSR.sprite;
        transform.position = Player.position;
        transform.rotation = Player.rotation;

        TimeActivated = Time.time;
    }
}
