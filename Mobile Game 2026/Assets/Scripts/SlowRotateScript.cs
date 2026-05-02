using UnityEngine;

public class SlowRotateScript : MonoBehaviour
{
    public bool randomStartRotation = true;

    public float rotationSpeed = 10f; // Degrees per second

    private void Start()
    {
        if (randomStartRotation)
            transform.eulerAngles = new Vector3(0, 0,Random.Range(0, 360));
    }

    void Update()
    {


        // Rotate around the Z-axis in local space
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}

