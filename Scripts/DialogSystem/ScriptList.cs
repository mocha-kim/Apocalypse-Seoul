using System;
using System.Collections.Generic;

namespace DialogSystem
{
    [Serializable]
    public class ScriptList
    {
        public int id;
        public Dictionary<int, Script> scripts = new();

        public ScriptList(int id)
        {
            this.id = id;
        }

        public void AddScript(int innerId, Script script) => scripts[innerId] = script;
    }

}