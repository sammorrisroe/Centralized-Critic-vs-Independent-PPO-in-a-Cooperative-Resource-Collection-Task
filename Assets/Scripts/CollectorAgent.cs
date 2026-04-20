using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class CollectorAgent : Agent
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private Transform goalTransform;

    [Header("Reset Control")]
    /*[SerializeField] private bool handlesEpisodeReset = false;*/

    private EpisodeManager episodeManager;
    private Rigidbody rb;
    private bool isCarrying = false;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetEpisodeManager(EpisodeManager manager)
    {
        episodeManager = manager;
    }
    /*
    public override void OnEpisodeBegin()
    {
        Debug.Log($"{name} episode begin at time {Time.time}");
        
        if (handlesEpisodeReset && episodeManager != null)
        {
            episodeManager.RequestReset();
        }
    }
    */
    public override void OnEpisodeBegin()
    {
        Debug.Log($"{name} episode begin at time {Time.time}");
    }


    public void ResetAgentState(Vector3 pos)
    {
        transform.position = pos;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isCarrying = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float arenaHalfSize = episodeManager != null ? episodeManager.GetArenaHalfSize() : 14f;

        sensor.AddObservation(transform.position.x / arenaHalfSize);
        sensor.AddObservation(transform.position.z / arenaHalfSize);

        Vector3 relGoal = goalTransform.position - transform.position;
        sensor.AddObservation(relGoal.x / arenaHalfSize);
        sensor.AddObservation(relGoal.z / arenaHalfSize);

        Vector3 relResource = resourceManager != null
            ? resourceManager.GetNearestResourceRelativePosition(transform.position)
            : Vector3.zero;
        sensor.AddObservation(relResource.x / arenaHalfSize);
        sensor.AddObservation(relResource.z / arenaHalfSize);

        Vector3 relTeammate = episodeManager != null
            ? episodeManager.GetOtherAgentRelativePosition(this)
            : Vector3.zero;
        sensor.AddObservation(relTeammate.x / arenaHalfSize);
        sensor.AddObservation(relTeammate.z / arenaHalfSize);

        sensor.AddObservation(isCarrying ? 1f : 0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (StepCount % 50 == 0)
{
            Debug.Log($"{name} StepCount = {StepCount}, Time = {Time.time}");
        }
        int moveAction = actions.DiscreteActions[0];
        Vector3 move = Vector3.zero;

        switch (moveAction)
        {
            case 0: move = Vector3.forward; break;
            case 1: move = Vector3.back;    break;
            case 2: move = Vector3.left;    break;
            case 3: move = Vector3.right;   break;
            case 4: move = Vector3.zero;    break;
        }

        rb.linearVelocity = new Vector3(
            move.x * moveSpeed,
            rb.linearVelocity.y,
            move.z * moveSpeed
        );

        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.W))
            discreteActions[0] = 0;
        else if (Input.GetKey(KeyCode.S))
            discreteActions[0] = 1;
        else if (Input.GetKey(KeyCode.A))
            discreteActions[0] = 2;
        else if (Input.GetKey(KeyCode.D))
            discreteActions[0] = 3;
        else
            discreteActions[0] = 4;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource") && !isCarrying)
        {
            isCarrying = true;
            episodeManager?.GiveCollectReward();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Goal") && isCarrying)
        {
            isCarrying = false;
            episodeManager?.GiveDepositReward();
        }
    }
}