using EventSystem;

namespace UI.FixedUI.EventUI
{
    public abstract class EventUIBase : UIBase
    {
        public override void Close()
        {
            EventManager.OnNext(Message.OnEventUIClosed);
            
            base.Close();
        }

        public abstract int Next(int curId);
    }
}