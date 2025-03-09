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

    private float elapsedTime = 0f;
    private float targetTime = 2f;

    private CancellationTokenSource cts;


    protected override void Awake()
    {
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }

    public override void Enter()
    {
        cts = new CancellationTokenSource();

        // ** 트럭에 상대적인 x좌표 **가 뒤로 이동해야하므로 현재 트럭과의 상대적인 x포지션을 받아옴
        originX = StateMachineOwner.gameObject.transform.localPosition.x;
        targetX = originX + 0.85f;

        MoveToBackwardRoutine().Forget();
    }

    public override void Exit()
    {
        cts?.Cancel();
        cts?.Dispose();
    }

    private async UniTask MoveToBackwardRoutine()
    {
        while (elapsedTime < targetTime)
        {
            await UniTask.Yield(cancellationToken: cts.Token);

            elapsedTime += Time.deltaTime;

            float newX = Mathf.Lerp(originX, targetX, elapsedTime/targetTime);

            // 만약 위에있던 몬스터일 경우 밑으로 떨어지면서 뒤로 가야하므로 x만 조정
            monsterCtrl.transform.localPosition = new Vector2(newX, monsterCtrl.transform.localPosition.y);
        }

        // 뒤로 이동이 끝나면 처음 상태로 전이
        StateMachine.ExecuteCommand(EMonsterStateCommand.Move);
    }
}
