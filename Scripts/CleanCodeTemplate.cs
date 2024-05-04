using UnityEngine;

namespace Alpha // namespace는 경로를 기준으로 함
{
    // 정리우선순위.
    // 1. 논리적그룹을 최우선으로 고려하여 정리한다.
    // 2. 공개적일수록 상단으로 올린다.
    // 3. 그외다른부분은 아래코드를 참고한다.
    
    public class CleanCodeTemplate : MonoBehaviour
    {
        #region ===================SerializeField===================

        [SerializeField] public double publicDouble;
        [SerializeField] private int inspectorNumber;

        #endregion

        #region ===================VARIABLE & PROPERTY===================
        public int publicNumber = 0;
        private int _month = 7;

        public int Month
        {
            get => _month;
            set
            {
                if ((value > 0) && (value < 13))
                {
                    _month = value;
                }
            }
        }

        #endregion

        #region ===================UNITY_EVENT===================

        private void Awake()
        {
            Debug.Log("Awake!");
        }

        private void Start()
        {
            Debug.Log("Start!");
        }

        private void OnDestroy()
        {
            Debug.Log("OnDestroy!");
        }

        #endregion

        #region ===================PUBLIC_METHOD===================

        public void PublicMethod()
        {
        }

        #endregion

        #region ===================PROTECTED_METHOD===================

        protected void ProtectedMethod()
        {
        }

        #endregion

        #region ===================PRIVATE_METHOD===================

        private void PrivateMethod()
        {
        }

        #endregion
    }
}