namespace UI
{
    public enum UIType
    {
        MainUI,
        SettingUI,
        SaveLoadUI,
        
        PlayerInventoryUI,
        StorageUI,
        ItemBoxUI,
        
        MapUI,
        ToastUI,
        
        ProducerUI,
        FieldProducerUI,
        ProducerUpgradeUI,
        
        EventUI,
        DialogUI,
        DealUI,
        
        // Forbidden to  call by UIManager
        ComponentUI = -1,
        ItemTooltipUI = -21,
    }
}