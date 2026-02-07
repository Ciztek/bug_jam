using UnityEngine;

public class bakery : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private float timer = 0f;
    private bool collisionEnabled = true;

    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= 1f)
        {
            timer = 0f;
            collisionEnabled = !collisionEnabled;
            GetComponent<MeshCollider>().enabled = collisionEnabled;
        }
    }
}
