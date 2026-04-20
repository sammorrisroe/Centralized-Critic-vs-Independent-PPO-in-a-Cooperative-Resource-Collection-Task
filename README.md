# Cooperative Resource Collection in Unity ML-Agents

This project compares:
- Independent PPO
- MA-POCA

## Requirements
- Unity version: 6000.0.33f1
- Python: 3.9
- ml-agents: 0.28.0
- ml-agents-envs: 0.28.0
- PyTorch: 1.8.1

## Project structure
- `Assets/`, `Packages/`, `ProjectSettings/`: Unity project
- `config/collector_ppo.yaml`: PPO training config
- `config/collector_poca.yaml`: POCA training config

## Training
Activate the Python environment, then run:

```bash
mlagents-learn config/collector_ppo.yaml --run-id=ppo_run1 --env="RLBuild.app" --no-graphics
mlagents-learn config/collector_poca.yaml --run-id=poca_run1 --env="RLBuildMulti.app" --no-graphics
