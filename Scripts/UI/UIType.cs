namespace UI
{
    public enum UIType
    {
        MainUI,
        SettingUI,
        
        PlayerInventoryUI,
        StorageUI,
        ItemBoxUI,
        
        MapUI,
        ToastUI,
        
        CraftTableUI,
        ProducerUpgradeUI,
        
        EventUI,
        DialogUI,
        DealUI,
        
        // Forbidden to  call by UIManager
        ComponentUI = -1,
        ItemTooltipUI = -21,
    }
}