using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class SelfDriver : Agent
{
    PrometeoCarController carCtrl;
    public GameObject Destination;
    Rigidbody rb;

    UnityEngine.Vector3 StartPosition;
    UnityEngine.Quaternion StartRotation;


    float PreviousDistance = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void OnEpisodeBegin()
    {
        Debug.Log("test");
        ResetState();
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var actions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.UpArrow))
            actions[0] = 1;
        else if (Input.GetKey(KeyCode.DownArrow))
            actions[0] = 2;
        else
            actions[0] = 0;

        if (Input.GetKey(KeyCode.LeftArrow))
            actions[1] = 1;
        else if (Input.GetKey(KeyCode.RightArrow))
            actions[1] = 2;
        else
            actions[1] = 0;
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-0.0001f);

        float CurrentDistance = Vector3.Distance(transform.position, Destination.transform.position);
        float Delta = PreviousDistance - Vector3.Distance(transform.position, Destination.transform.position);

        if (PreviousDistance != 0)
            AddReward(0.01f * Delta);

        PreviousDistance = CurrentDistance;



        var action = actions.DiscreteActions;

        if (action[0] == 1)
            carCtrl.GoForward();
        else if (action[0] == 2)
            carCtrl.GoReverse();
        else
            carCtrl.ThrottleOff();
        if (action[1] == 1)
            carCtrl.TurnLeft();
        else if (action[1] == 2)
            carCtrl.TurnRight();
        else
            carCtrl.ResetSteeringAngle();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 CarPosition = transform.position;
        Vector3 DestinatonPosition = Destination.transform.position;
        Vector3 CarDirection = transform.forward;
        Vector3 CarSpeed = rb.linearVelocity;

        sensor.AddObservation(CarPosition);
        sensor.AddObservation(DestinatonPosition);
        sensor.AddObservation(CarDirection);
        sensor.AddObservation(CarSpeed);
    }


    void ResetState()
    {
        transform.position = StartPosition;
        transform.rotation = StartRotation;
        carCtrl.ResetCar();
    }

    void Start()
    {
        carCtrl = GetComponent<PrometeoCarController>();
        rb = GetComponent<Rigidbody>();
        StartPosition = transform.position;
        StartRotation = transform.rotation;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Cone")
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Destination")
        {
            Debug.Log("SUCCESS!!!");
            AddReward(1.0f);
            EndEpisode();
        }      
    }

}
