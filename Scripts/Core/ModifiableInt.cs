using System;
using Newtonsoft.Json;

namespace Core
{
    [Serializable]
    public class ModifiableInt
    {
        [JsonProperty] private int _baseValue;
        [JsonProperty] private int _modifiedValue;

        public int BaseValue
        {
            get => _baseValue;
            set
            {
                _modifiedValue += _baseValue - value;
                _baseValue = value;
            }
        }

        public int ModifiedValue
        {
            get => _modifiedValue;
            set => _modifiedValue = value < 0 ? 0 : value;
        }
        
        public ModifiableInt()
        {
            _baseValue = 0;
            _modifiedValue = 0;
        }

        public ModifiableInt(int value)
        {
            _baseValue = value;
            _modifiedValue = value;
        }

        public void InitValue(int value)
        {
            _baseValue = value;
            _modifiedValue = value;
        }

        public void ResetModifiedValue()
        {
            _modifiedValue = _baseValue;
        }
    }
}