using UnityEngine;

public class PlatformerCamera : MonoBehaviour
{
    public GameObject subject;
    public float windowSize;
    public float minY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        subject = GameObject.Find("NotSamus");
    }

    // Update is called once per frame
    void Update()
    {
       try
        {
            Vector3 newPosition = new Vector3(Mathf.Clamp(transform.position.x, subject.transform.position.x - windowSize, subject.transform.position.x + windowSize),
                                            Mathf.Max(minY, subject.transform.position.y + 1), -10);
            transform.position = newPosition;
        }
        catch
        {
            subject = GameObject.Find("NotSamus");
        }
       
    }
}
