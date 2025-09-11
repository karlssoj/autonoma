using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CleanerController : Agent
{
    public float Speed = 1;
    public float TurnSpeed = 10;
    Vector3 StartPosition;
    public GameObject Dust;



    public override void OnEpisodeBegin()
    {
        Reset();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        if (action[0] == 1)
            MoveForward();
        if (action[0] == 2)
            MoveBackwards();
        if (action[1] == 1)
            TurnRight();
        if (action[1] == 2)
            TurnLeft();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.UpArrow)) action[0] = 1;
        else if (Input.GetKey(KeyCode.DownArrow)) action[0] = 2;

        if (Input.GetKey(KeyCode.LeftArrow)) action[1] = 2;
        else if (Input.GetKey(KeyCode.RightArrow)) action[1] = 1;      
    }

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
        Dust.transform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), Dust.transform.localPosition.y, Random.Range(-4.0f, 4.0f));
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
            AddReward(-1.0f);
            EndEpisode();
        }

        if (collision.gameObject.tag == "Dust")
        {
            Debug.Log("SUCCESS!");
            AddReward(1.0f);
            EndEpisode();
        }
    }
}
