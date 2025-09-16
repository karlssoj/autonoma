using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CleanerController : Agent
{
    public float Speed = 1;
    public float TurnSpeed = 10;
    Vector3 StartPosition;
    public GameObject Dust;
    float PreviousDistance = 0;



    public override void OnEpisodeBegin()
    {
        Reset();
    }

	//Anropas när en action tas emot från "hjärnan" (om en intränad modell används på agenten)/trainer APIt (vid träning)/Heurustic-mode 
	//(om man kör simulationen i människostyrt läge)     
    public override void OnActionReceived(ActionBuffers actions)
    {
        //Nuvarande avstånd till skräp
        float CurrentDistance = Vector3.Distance(transform.localPosition, Dust.transform.localPosition);

        //Föregående asvstånd minus nuvarande avstånd blir positivt om vi kommit närmare skräpet
        float Delta = PreviousDistance - CurrentDistance;

        if(PreviousDistance != 0)
            //Pluspoäng om vi kommit närmare skräpet, minuspoäng om vi gått längre bort från skräpet
            AddReward(Delta * 0.01f);

        //Uppdaterar previousDistance
        PreviousDistance = CurrentDistance;

        //Varje beslut bör "kosta" lite, detta för att lära agenten att så snabbt som möjligt hitta skräpet
        AddReward(-0.0001f);

        //En Array med disreta actions. Två element med värde 0, 1, eller 2
        //Matchar med de actions man definierat i Beahiour Parameters skriptet
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

	//Anropas får varje frame om man kör simulationen i Heuristic mode (människostyrt läge), dvs. om vi inte har 
	//Python trainer API igång och om vi inte har kopplat en intränad modell till agenten
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;

		//Här ger användaren värden för actions-buffern manuellt under körning. Dessa värden tas som input i
		//OnActionsReceived
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

	//Sätter damsugaren på sin ursprungliga position och slumpar ut en ny koordinat för skräpet
    void Reset()
    {
        transform.position = StartPosition;
        SpawnDust();
    }


    void SpawnDust()
    {
        Dust.transform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f), Dust.transform.localPosition.y, Random.Range(-4.0f, 4.0f));
    }

	//Rörelsefunktioner för damsugaren
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
			
			//Om damsugaren krockar med väggen får den -1p och så startar vi om träningsepisoden
			//EndEpoisode triggar OnEpisodeBegin funtionen
            AddReward(-1.0f);
            EndEpisode();
        }

        if (collision.gameObject.tag == "Dust")
        {
            Debug.Log("SUCCESS!");
			
			//Om damsugaren nuddar vid skräpet får den +1p och så startar vi om träningsepisoden
			//EndEpoisode triggar OnEpisodeBegin funtionen
            AddReward(1.0f);
            EndEpisode();
        }
    }
}
