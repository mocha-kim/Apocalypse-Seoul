using CharacterSystem.Character.Combat;
using UnityEditor;
using UnityEngine;

namespace CharacterSystem.Character.Editor
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : UnityEditor.Editor
    {
        private void OnSceneGUI()
        {
            var fov = (FieldOfView)target;
            var position = fov.transform.position;
            
            var viewAngleA = Quaternion.AngleAxis(-fov.ViewAngle / 2, Vector3.forward) * fov.Forward;
            var viewAngleB = Quaternion.AngleAxis(fov.ViewAngle / 2, Vector3.forward) * fov.Forward;

            Handles.color = Color.white;
            Handles.DrawWireArc(position, Vector3.forward, viewAngleA, fov.ViewAngle, fov.ViewRadius);
            
            Handles.DrawLine(position, position + viewAngleA * fov.ViewRadius);
            Handles.DrawLine(position, position + viewAngleB * fov.ViewRadius);

            Handles.color = Color.red;
            if (fov.ClosestTarget != null)
            {
                Handles.DrawLine(fov.transform.position, fov.ClosestTarget.position); 
            }
        }
    }
}