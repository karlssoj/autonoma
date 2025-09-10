using UnityEngine;

public class CleanerController : MonoBehaviour
{
    public float Speed = 1;
    public float TurnSpeed = 10;
    Vector3 StartPosition;
    public GameObject Dust;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPosition = transform.position;
        Reset();
    }


    void Reset()
    {
        transform.position = StartPosition;
        SpawnDust();
    }


    void SpawnDust()
    {
        Dust.transform.position = new Vector3(Random.Range(-4.8f, 4.8f), Dust.transform.position.y, Random.Range(-4.8f, 4.8f));
    }

    // Update is called once per frame
    void Update()
    {
        SteeringControl();
    }

    void SteeringControl()
    {
        if (Input.GetKey(KeyCode.UpArrow)) MoveForward();
        else if (Input.GetKey(KeyCode.DownArrow)) MoveBackwards();

        if (Input.GetKey(KeyCode.LeftArrow)) TurnLeft();
        else if (Input.GetKey(KeyCode.RightArrow)) TurnRight();
    }

    void MoveForward()
    {
        transform.Translate(0, 0, Speed * Time.deltaTime);
    }

    void MoveBackwards()
    {
        transform.Translate(0, 0, -Speed * Time.deltaTime);
    }

    void TurnRight()
    {
        transform.Rotate(0, TurnSpeed * Time.deltaTime, 0);
    }

    void TurnLeft()
    {
        transform.Rotate(0, -TurnSpeed * Time.deltaTime, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log("FAILURE!");
            Reset();
        }

        if (collision.gameObject.tag == "Dust")
        {
            Debug.Log("SUCCESS!");
            Reset();
        }
    }
}
