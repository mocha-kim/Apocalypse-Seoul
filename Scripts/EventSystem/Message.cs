namespace EventSystem
{
    public enum Message
    {
        OnReadyGameManager,
        
        // Scene Load
        OnTrySceneLoad,
        OnSceneLoadComplete,
        
        // Time
        OnEveryHour,
        OnEveryMinute,
        
        // State & Combat
        OnPlayerAttributeChanged,
        OnPlayerAffected,
        OnPlayerEffectChanged,
        OnPlayerDead,
        
        OnCharacterBusy,
        OnCharacterFree,
        
        OnPlayerAimStart,
        OnPlayerAimEnd,
        
        OnCancelRunning,
        
        OnEnemyDamaged,
        
        
        // UI
        OnUIOpened,
        OnUIClosed,
        
        OnEventUIClosed,
        OnNextEventUI,
        OnAnswerSelected,
        
        OnReadyMapMove,
        OnMapSelect,
        
        OnTryTooltipOpen,
        OnTryTooltipClose,
        OnRefreshAllBindingUIs,

        // Item
        OnUpdateInventory,
        OnTryItemUse,
        OnItemUsed,
        
        // Produce
        OnClickProductNode,
        OnClickProduce,
        
        // Producer upgrade
        OnTryProducerUpgrade,
        OnProducerUpgraded,
        OnClickProducerNode,
        
        //Deal
        OnDealUIInit,
        OnDealUILink,
        OnDoDeal,
        OnResetDeal,
        
        // Env
        OnAreaEnter,
        OnAreaExit,
        
        // Input
        OnTryInteract,
        OnPressEscape,
        OnLeftMouseDown,
        
        OnRightMouseDown,
        OnRightMouseUp,
        
        OnPressChangeWeapon,
        
        // Data
        OnDataSaved,
        OnDataLoaded,
    }
}