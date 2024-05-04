using CharacterSystem.Character;
using CharacterSystem.Stat;

namespace Core.Interface
{
    public interface IAffectable
    {
        public void Affect(AttributeType type, int value);
        public int GetReferenceValue(AttributeType type);
        public CharacterType CharacterType { get; }
    }
}