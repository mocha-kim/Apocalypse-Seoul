using CharacterSystem.Character;
using CharacterSystem.Effect;
using CharacterSystem.Stat;

namespace Core.Interface
{
    public interface IAffectable
    {
        public void Affect(AttributeType type, int value);
        public void Affect(Effect effect);
        
        public int GetReferenceValue(AttributeType type);
        public CharacterType CharacterType { get; }
    }
}