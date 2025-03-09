using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.UI.Image;

public class MoveToBackwardState : State<Monster>
{
    private MonsterCtrl monsterCtrl;

    private float originX;
    private float targetX;

    private float elapsedTime;
    private float targetTime;

    private CancellationTokenSource cts;


    protected override void Awake()
    {
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }

    public override void Enter()
    {
        cts = new CancellationTokenSource();
        elapsedTime = 0f;
        targetTime = 1.5f;

        // ** Ʈ���� ������� x��ǥ **�� �ڷ� �̵��ؾ��ϹǷ� ���� Ʈ������ ������� x�������� �޾ƿ�
        originX = StateMachineOwner.gameObject.transform.localPosition.x;
        targetX = originX + 1f;

        MoveToBackwardRoutine().Forget();
    }

    public override void Exit()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private async UniTask MoveToBackwardRoutine()
    {
        monsterCtrl.Rigid.mass = 100f;

        while (elapsedTime < targetTime)
        {
            await UniTask.Yield(cancellationToken: cts.Token);

            elapsedTime += Time.deltaTime;

            float newX = Mathf.Lerp(originX, targetX, elapsedTime/targetTime);

            // ���� �����ִ� ������ ��� ������ �������鼭 �ڷ� �����ϹǷ� x�� ����
            monsterCtrl.transform.localPosition = new Vector2(newX, monsterCtrl.transform.localPosition.y);
        }

        monsterCtrl.Rigid.mass = 1f;

        // �ڷ� �̵��� ������ ó�� ���·� ����
        StateMachine.ExecuteCommand(EMonsterStateCommand.Move);
    }
}
