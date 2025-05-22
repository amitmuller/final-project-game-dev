// Scripts/EnemyAI/EnemyState.cs
using UnityEngine;

namespace EnemyAI
{
    public enum EnemyStateType
    {
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
    }
}