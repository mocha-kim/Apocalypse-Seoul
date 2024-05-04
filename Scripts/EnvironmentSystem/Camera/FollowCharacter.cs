using UnityEngine;

namespace EnvironmentSystem.Camera
{
    public class FollowCharacter : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool isFollow = true;
        [SerializeField] private bool isSmooth = true;
        [SerializeField] [Range(0.01f, 0.1f)] private float smoothSpeed = 0.01f;
        [SerializeField] private float boundX = 1f;
        [SerializeField] private float boundY = 1f;

        private void FixedUpdate()
        {
            if (!isFollow)
            {
                return;
            }

            Vector2 targetPos = target.position;
            Vector2 newPos = transform.position;
            if (isSmooth)
            {
                newPos = Vector3.Lerp(newPos, targetPos, smoothSpeed);    
            }
            

            // check if inside boundX
            var deltaX = targetPos.x - newPos.x;
            if (deltaX > boundX || deltaX < -boundX)
            {
                if (newPos.x < targetPos.x)
                {
                    newPos.x = targetPos.x - boundX;
                }
                else
                {
                    newPos.x = targetPos.x + boundX;
                }
            }
            
            // check if inside boundY
            var deltaY = targetPos.y - newPos.y;
            if (deltaY > boundY || deltaY < -boundY)
            {
                if (newPos.y < targetPos.y)
                {
                    newPos.y = targetPos.y - boundY;
                }
                else
                {
                    newPos.y = targetPos.y + boundY;
                }
            }
            
            // setPos camera
            transform.position = new Vector3(newPos.x, newPos.y, -10);
        }

        public void InitTarget(Transform transform) => target = transform;
        
#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            //디버그용.
            Vector3 curPos = transform.position;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(curPos + new Vector3(-boundX, -boundY, 0), curPos + new Vector3(-boundX, boundY, 0));
            Gizmos.DrawLine(curPos + new Vector3(-boundX, -boundY, 0), curPos + new Vector3(boundX, -boundY, 0));
            Gizmos.DrawLine(curPos + new Vector3(boundX, boundY, 0), curPos + new Vector3(boundX, -boundY, 0));
            Gizmos.DrawLine(curPos + new Vector3(boundX, boundY, 0), curPos + new Vector3(-boundX, boundY, 0));
        }
#endif
    }
}