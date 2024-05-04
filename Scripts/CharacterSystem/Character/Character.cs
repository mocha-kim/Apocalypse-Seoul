using UnityEngine;

namespace CharacterSystem.Character
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer _render;
        public abstract CharacterType CharacterType { get; }
    }
}