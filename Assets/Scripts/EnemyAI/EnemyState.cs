// Scripts/EnemyAI/EnemyState.cs
using UnityEngine;

namespace EnemyAI
{
    public enum EnemyStateType
    {
        PatrolIdle, // Patrol idle is when the enemy is idling between travelling patrol points
        PermanentIdle, // Permanent idle is when the enemy is not moving at all
        Calm,
        Alert,
        Searching,
        Chase
    }

    public interface IEnemyState
    {
        EnemyStateType StateType { get; }
        void EnterState(EnemyAIController enemy);
        void UpdateState(EnemyAIController enemy);
        void ExitState(EnemyAIController enemy);
        
        void OnNoiseRaised(Vector2 noisePosition, EnemyAIController enemy) { } // don't need to implement in all states

    }
}