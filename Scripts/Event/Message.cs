namespace Event
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
        OnPlayerEffectChanged,
        
        OnCharacterBusy,
        OnCharacterFree,
        
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
        
        // Input
        OnTryInteract,
        OnPressEscape,
        OnLeftMouseDown,
        
        OnRightMouseDown,
        OnRightMouseUp,
        
        OnPressChangeWeapon,
    }
}