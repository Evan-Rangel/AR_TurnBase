using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAction : BaseAction
{
    public static event EventHandler OnAnyKnifeHit;
    public event EventHandler OnKnifeActionStarted;
    public event EventHandler OnKnifeActionCompleted;

    private enum State
    {
        SwingingKnifeBeforeHit,
        SwingingKnifeAfterHit,
    }

    private int maxKnifeDistance = 1;
    [SerializeField] private int knifeDamage = 100;
    [SerializeField] private int actionCost = 2;
    private State state;
    private float stateTimer;
    private Unit targetUnit;

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingKnifeBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingKnifeAfterHit:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwingingKnifeBeforeHit:
                state = State.SwingingKnifeAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(knifeDamage);
                OnAnyKnifeHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingKnifeAfterHit:
                OnKnifeActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxKnifeDistance; x <= maxKnifeDistance; x++)
        {
            for (int z = -maxKnifeDistance; z <= maxKnifeDistance; z++)
            {
                //ridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    continue;
                }

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (targetUnit.IsEnemy() == unit.IsEnemy())
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.SwingingKnifeBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnKnifeActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance()
    {
        return maxKnifeDistance;
    }

    public override int GetActionPointsCost()
    {
        return actionCost;
    }
}
