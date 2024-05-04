using System;
using Event;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Manager
{
    public class EventManager
    {
        private static Subject<Event> _event = new();

        public static void OnNext(Message message, params object[] args)
        {
            _event.OnNext(new Event(message, args));
        }

        public static IDisposable Subscribe(GameObject go, Message message, Action<Event> onEvent)
        {
            return _event
                .Where(e => e.Message == message)
                .Subscribe(e => { onEvent(e); }).AddTo(go);
        }
        
        public static void AddEventTrigger(GameObject go, EventTriggerType triggerType, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if (!trigger)
            {
                Debug.LogWarning(go.name + " has no EventTrigger component");
                return;
            }

            foreach (var entry in trigger.triggers)
            {
                if (entry.eventID == triggerType)
                {
                    entry.callback.AddListener(action);
                    return;
                }
            }
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = triggerType };
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public class Event
        {
            public Message Message { get; private set; }
            public object[] Args { get; private set; }

            public Event(Message message, params object[] args)
            {
                this.Message = message;
                this.Args = args;
            }
        }
    }
}


//샘플코드를 여기에 남겨놓음.
/*
public class Example : MonoBehaviour
{
    //이벤트를 생성.
    void OnNext()
    {
        //#Case1. only event.
        EventManager.OnNext(EMessage.Test);


        //#Case2. use params.
        EventManager.OnNext(EMessage.Test, 1);
    }


    //이벤트발생을 감지.
    void Subscribe()
    {
        //#Case1. only event.
        EventManager.Subscribe(gameObject, EMessage.Test, _ =>
        {
            Debug.Log("Test event detected.");
        });


        //#case 2. use params.
        EventManager.Subscribe(gameObject, EMessage.Test, e =>
        {
            int temp = (int)e.Args[0];//값을 넘겨주지않으면, 에러발생.
            Debug.Log($"Test event detected. temp: [{temp}]");
        });
    }
}

*/