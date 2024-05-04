using System;
using CharacterSystem.Character;
using UniRx;
using UnityEngine;

namespace Event
{
    public static class ObservableTriggerExtensions
    {
        public static IObservable<MoveDir> Press8DirObservable(this Component component)
        {
            if (component == null || component.gameObject == null) return Observable.Empty<MoveDir>();
            return GetOrAddComponent<ObservablePress8DirTrigger>(component.gameObject).Press8DirObservable();
        }

        private static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }

}