using System;
using InputSystem.UserActionBind;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace InputSystem.InputTrigger
{
    public class ObservablePress8DirTrigger : ObservableTriggerBase
    {
        private Subject<MoveDir> _subject;

        public IObservable<MoveDir> Press8DirObservable()
        {
            return _subject ??= new Subject<MoveDir>();
        }

        private void Update()
        {
            var pressUp = Input.GetKey(InputBinding.Bindings[UserAction.MoveUp]);
            var pressDown = Input.GetKey(InputBinding.Bindings[UserAction.MoveDown]);
            var pressLeft = Input.GetKey(InputBinding.Bindings[UserAction.MoveLeft]);
            var pressRight = Input.GetKey(InputBinding.Bindings[UserAction.MoveRight]);

            var xAxis = 0;
            if (pressLeft ^ pressRight)
            {
                xAxis = pressLeft ? -1 : 1;
            }

            var yAxis = 0;
            if (pressUp ^ pressDown)
            {
                yAxis = pressDown ? -1 : 1;
            }


            var pressDir = new Vector2Int(xAxis, yAxis);

            var newDir = pressDir switch
            {
                _ when pressDir == Vector2Int.zero => MoveDir.None,
                _ when pressDir == Vector2Int.up => MoveDir.Up,
                _ when pressDir == Vector2Int.down => MoveDir.Down,
                _ when pressDir == Vector2Int.left => MoveDir.Left,
                _ when pressDir == Vector2Int.right => MoveDir.Right,
                _ when pressDir == Vector2Int.up + Vector2Int.left => MoveDir.UpLeft,
                _ when pressDir == Vector2Int.up + Vector2Int.right => MoveDir.UpRight,
                _ when pressDir == Vector2Int.down + Vector2Int.left => MoveDir.DownLeft,
                _ when pressDir == Vector2Int.down + Vector2Int.right => MoveDir.DownRight,
                _ => MoveDir.None
            };

            _subject.OnNext(newDir);
        }

        protected override void RaiseOnCompletedOnDestroy()
        {
            _subject?.OnCompleted();
        }
    }
}