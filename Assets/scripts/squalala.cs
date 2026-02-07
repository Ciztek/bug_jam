using UnityEngine;

public class squalala : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite sprite1;
    public Sprite sprite2;
    public Sprite sprite3;
    private float timer = 0f;
    private int currentSpriteIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //c'est un sprite en 2D, je veux changer son sprite pour faire une animation
        timer += Time.deltaTime;
        if (timer >= 0.3f)
        {
            timer = 0f;
            currentSpriteIndex = (currentSpriteIndex + 1) % 3; // Cycle through 3 sprites
            switch (currentSpriteIndex)
            {
                case 0:
                    spriteRenderer.sprite = sprite1;
                    break;
                case 1:
                    spriteRenderer.sprite = sprite2;
                    break;
                case 2:
                    spriteRenderer.sprite = sprite3;
                    break;
            }
        }
    }
}
