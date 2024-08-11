using InputSystem;
using ItemSystem.Item;

namespace UI.FloatingUI.Tooltip.Target
{
    public class ItemTooltipTarget : TooltipTarget<Item>
    {
        protected override Item GetTarget() => MouseData.MouseHoveredSlot.Item;
    }
}