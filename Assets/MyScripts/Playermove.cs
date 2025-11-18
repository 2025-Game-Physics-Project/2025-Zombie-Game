using UnityEngine;

public class Playermove : MonoBehaviour
{
    public float moveSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal"); // A,D 또는 ←,→
        float z = Input.GetAxis("Vertical");   // W,S 또는 ↑,↓

        Vector3 move = new Vector3(x, 0, z);

        transform.Translate(move * moveSpeed * Time.deltaTime);
    }
}
