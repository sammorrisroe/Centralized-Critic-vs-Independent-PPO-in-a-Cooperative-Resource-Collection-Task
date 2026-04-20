/*
using System.Collections.Generic;
using UnityEngine;

public class EpisodeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private List<CollectorAgent> agents = new List<CollectorAgent>();

    [Header("Reset Positions")]
    [SerializeField] private Vector3 agent1ResetPosition = new Vector3(-.75f, 1.5f, 0f);
    [SerializeField] private Vector3 agent2ResetPosition = new Vector3(.75f, 1.5f, 0f);

    [Header("Arena Bounds")]
    [SerializeField] private float arenaMinX = -14f;
    [SerializeField] private float arenaMaxX = 14f;
    [SerializeField] private float arenaMinZ = -14f;
    [SerializeField] private float arenaMaxZ = 14f;

    [Header("Rewards")]
    [SerializeField] private float collectReward = 0.10f;
    [SerializeField] private float depositReward = 1.00f;

    private bool resetRequested = false;

    private void Start()
    {
        foreach (CollectorAgent agent in agents)
        {
            if (agent != null)
            {
                agent.SetEpisodeManager(this);
            }
        }

        ResetEnvironment();
    }

    private void LateUpdate()
    {
        if (!resetRequested) return;

        resetRequested = false;
        ResetEnvironment();
        Debug.Log("Shared environment reset");
    }

    public void RequestReset()
    {
        resetRequested = true;
    }

    public void ResetEnvironment()
    {
        if (resourceManager != null)
        {
            resourceManager.ResetResources();
        }

        if (agents.Count > 0 && agents[0] != null)
        {
            agents[0].ResetAgentState(agent1ResetPosition);
        }

        if (agents.Count > 1 && agents[1] != null)
        {
            agents[1].ResetAgentState(agent2ResetPosition);
        }
    }

    public void GiveCollectReward()
    {
        foreach (CollectorAgent agent in agents)
        {
            if (agent != null)
            {
                agent.AddReward(collectReward);
            }
        }
    }

    public void GiveDepositReward()
    {
        foreach (CollectorAgent agent in agents)
        {
            if (agent != null)
            {
                agent.AddReward(depositReward);
            }
        }
    }

    public Vector3 GetOtherAgentRelativePosition(CollectorAgent self)
    {
        foreach (CollectorAgent agent in agents)
        {
            if (agent != null && agent != self)
            {
                return agent.transform.position - self.transform.position;
            }
        }

        return Vector3.zero;
    }

    public float GetArenaHalfSize()
    {
        float halfX = Mathf.Max(Mathf.Abs(arenaMinX), Mathf.Abs(arenaMaxX));
        float halfZ = Mathf.Max(Mathf.Abs(arenaMinZ), Mathf.Abs(arenaMaxZ));
        return Mathf.Max(halfX, halfZ);
    }
}
*/

using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class EpisodeManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ResourceManager resourceManager;
    [SerializeField] private List<CollectorAgent> agents = new List<CollectorAgent>();

    [Header("Reset Positions")]
    [SerializeField] private Vector3 agent1ResetPosition = new Vector3(-.75f, 1.5f, 0f);
    [SerializeField] private Vector3 agent2ResetPosition = new Vector3(.75f, 1.5f, 0f);

    [Header("Arena Bounds")]
    [SerializeField] private float arenaMinX = -14f;
    [SerializeField] private float arenaMaxX = 14f;
    [SerializeField] private float arenaMinZ = -14f;
    [SerializeField] private float arenaMaxZ = 14f;

    [Header("Rewards")]
    [SerializeField] private float collectReward = 0.10f;
    [SerializeField] private float depositReward = 1.00f;

    [Header("Episode Control")]
    [SerializeField] private int maxEnvironmentSteps = 3000;

    private bool resetRequested = false;
    private int environmentStepCount = 0;

    private SimpleMultiAgentGroup agentGroup;

    private void Start()
    {
        agentGroup = new SimpleMultiAgentGroup();

        foreach (CollectorAgent agent in agents)
        {
            if (agent != null)
            {
                agent.SetEpisodeManager(this);
                agentGroup.RegisterAgent(agent);
            }
        }

        ResetEnvironment();
    }

    private void FixedUpdate()
    {
        environmentStepCount++;

        if (environmentStepCount >= maxEnvironmentSteps)
        {
            agentGroup.GroupEpisodeInterrupted();
            ResetEnvironment();
        }
    }

    private void LateUpdate()
    {
        if (!resetRequested) return;

        resetRequested = false;

        agentGroup.EndGroupEpisode();
        ResetEnvironment();

        Debug.Log("Shared environment reset");
    }

    public void RequestReset()
    {
        resetRequested = true;
    }

    public void ResetEnvironment()
    {
        environmentStepCount = 0;

        if (resourceManager != null)
        {
            resourceManager.ResetResources();
        }

        if (agents.Count > 0 && agents[0] != null)
        {
            agents[0].ResetAgentState(agent1ResetPosition);
        }

        if (agents.Count > 1 && agents[1] != null)
        {
            agents[1].ResetAgentState(agent2ResetPosition);
        }
    }

    public void GiveCollectReward()
    {
        agentGroup.AddGroupReward(collectReward);
    }

    public void GiveDepositReward()
    {
        agentGroup.AddGroupReward(depositReward);

    }

    public Vector3 GetOtherAgentRelativePosition(CollectorAgent self)
    {
        foreach (CollectorAgent agent in agents)
        {
            if (agent != null && agent != self)
            {
                return agent.transform.position - self.transform.position;
            }
        }

        return Vector3.zero;
    }

    public float GetArenaHalfSize()
    {
        float halfX = Mathf.Max(Mathf.Abs(arenaMinX), Mathf.Abs(arenaMaxX));
        float halfZ = Mathf.Max(Mathf.Abs(arenaMinZ), Mathf.Abs(arenaMaxZ));
        return Mathf.Max(halfX, halfZ);
    }
}