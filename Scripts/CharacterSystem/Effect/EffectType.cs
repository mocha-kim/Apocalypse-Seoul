namespace CharacterSystem.Effect
{
    public enum EffectType
    {
        DefaultHunger,      // Player Default Effect, decrease hunger
        DefaultThirst,      // Player Default Effect, decrease thirst
        
        ItemEffect,         // Item Effect Common Type, Not Unique in Database

        #region Negative Effects

        Starving = 100,
        Thirsty,
        
        Running,

        #endregion
        
        #region Positive Effects

        Full = 200,
        Moderate,
        Hydrated,
        
        Heal20,

        #endregion

    }
}