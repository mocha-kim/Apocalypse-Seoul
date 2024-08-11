using CharacterSystem.Character.Combat;
using CharacterSystem.Character.Combat.AttackBehavior;
using CharacterSystem.Character.Enemy;
using CharacterSystem.Character.Player;
using UnityEditor;
using UnityEngine;

namespace CharacterSystem.Character.Editor
{
    [CustomEditor(typeof(DefaultEnemyCharacter))]
    public class EnemyCharacterEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var character = (DefaultEnemyCharacter)target;
            var position = character.transform.position;
            var forward = character.Forward;

            switch (character.CurrentAttackBehavior)
            {
                case RangeSingleAttackBehavior rangeBehavior:
                    var viewAngleA = Quaternion.AngleAxis(-rangeBehavior.angle / 2, Vector3.forward) * forward;
                    var viewAngleB = Quaternion.AngleAxis(rangeBehavior.angle / 2, Vector3.forward) * forward;

                    Handles.color = Color.red;
                    Handles.DrawWireArc(position
                        , Vector3.forward
                        , viewAngleA
                        , rangeBehavior.angle
                        , rangeBehavior.range);

                    Handles.DrawLine(position, position + viewAngleA * rangeBehavior.range);
                    Handles.DrawLine(position, position + viewAngleB * rangeBehavior.range);
                    break;
            }
        }
    }
}