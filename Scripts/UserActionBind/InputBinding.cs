using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

// ref : https://rito15.github.io/posts/input-binding-system/

namespace UserActionBind
{
    public class InputBinding
    {
        public static Dictionary<UserAction, KeyCode> Bindings { get; set; } = new();

        public static void Init()
        {
            SetDefaultBind();
        }
        
        //중복이면 기존 바인딩 제거후, 진행.
        public static void Bind(UserAction action, KeyCode code)
        {
            var copy = new Dictionary<UserAction, KeyCode>(Bindings);
            //중복제거.
            foreach (var pair in copy)
            {
                if (pair.Value.Equals(code))
                {
                    Bindings[pair.Key] = KeyCode.None;
                }
            }

            //바인딩.
            Bindings[action] = code;
        }

        public static void SetDefaultBind()
        {
            Bindings.Clear();
            Bind(UserAction.MoveUp, KeyCode.W);
            Bind(UserAction.MoveDown, KeyCode.S);
            Bind(UserAction.MoveLeft, KeyCode.A);
            Bind(UserAction.MoveRight, KeyCode.D);
            Bind(UserAction.Run, KeyCode.LeftShift);

            Bind(UserAction.Inventory, KeyCode.Tab);
            Bind(UserAction.Interact, KeyCode.E);
            Bind(UserAction.Escape, KeyCode.Escape);
            Bind(UserAction.QuickSlot1, KeyCode.Alpha1);
            Bind(UserAction.QuickSlot2, KeyCode.Alpha2);
            Bind(UserAction.QuickSlot3, KeyCode.Alpha3);
            
            Bind(UserAction.Function, KeyCode.LeftControl);
            
            Bind(UserAction.ChangeWeapon, KeyCode.Q);
        }
    }
}