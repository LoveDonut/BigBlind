using EnemyState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMoverMovement : EnemyMovement
{
    #region PrivateVariables
    [Header("RandomMove")]
    [SerializeField] float _randomMoveDistance = 2f;
    [SerializeField] float _randomMoveCooldown = 5f;
    [SerializeField] float _randomMoveDuration = 2f;
    [SerializeField] float _randomSpeedMultiplier = 1.5f;
    float _randomMoveCooldownOffset = 1.5f;
    float _randomMoveDistanceOffset = 1.5f;
    #endregion

    #region PrivateMethods
    protected override void Start()
    {
        base.Start();

        StartCoroutine(RandomMoveCoroutine());
    }

    IEnumerator RandomMoveCoroutine()
    {
        while (true)
        {
            float cooldown = Random.Range(_randomMoveCooldown / _randomMoveCooldownOffset,
                                        _randomMoveCooldown * _randomMoveCooldownOffset);

            float distance = Random.Range(_randomMoveDistance / _randomMoveDistanceOffset, 
                                        _randomMoveDistance * _randomMoveDistanceOffset);

            yield return new WaitForSeconds(cooldown);

            if (CurrentState.GetType() == typeof(ChaseState))
            {
                MoveToDestState moveToDestState = new MoveToDestState();
                moveToDestState.SpeedMultiplier = _randomSpeedMultiplier;
                moveToDestState.MoveDistance = distance;
                moveToDestState.MoveDuration = _randomMoveDuration;

                CurrentState.SwitchState(gameObject, ref CurrentState, moveToDestState);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(CurrentState is MoveToDestState)
        {
            MoveToDestState moveToDestState = CurrentState as MoveToDestState;
            moveToDestState.IsCollided = true;
        }
    }
    #endregion
}
