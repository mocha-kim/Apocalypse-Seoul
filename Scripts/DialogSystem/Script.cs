using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogSystem
{
    [Serializable]
    public class Script
    {
        public int id;
        public ScriptType type;
        
        public int speakerId = -1;
        public bool isRightSpeaker;

        public int nextId;
        public string script;
        public Dictionary<string, int> answers;
        
        public Script(int id, ScriptType type, int nextId)
        {
            this.id = id;
            this.type = type;
            this.nextId = nextId;

            if (type == ScriptType.Dialog)
            {
                Debug.LogWarning("Created Dialog Script, but there is no dialog info");
            }
        }

        public Script(int id, ScriptType type, int nextId, int speakerId, bool isRightSpeaker, string script)
        {
            this.id = id;
            this.type = type;
            this.nextId = nextId;
            
            this.speakerId = speakerId;
            this.isRightSpeaker = isRightSpeaker;
            
            this.script = script;

            if (type != ScriptType.Dialog)
            {
                Debug.LogWarning("Created Non-Dialog Script, but this has dialog info");
            }
        }

        public void AddAnswer(string answer, int nextInnerId)
        {
            answers ??= new Dictionary<string, int>();
            answers[answer] = nextInnerId;
        }
    }

}