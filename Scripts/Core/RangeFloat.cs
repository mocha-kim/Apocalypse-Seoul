using System;

namespace Core
{
    [Serializable]
    public struct RangeFloat
    {
        public float start;
        public float end;

        public RangeFloat(float start, float end)
        {
            this.start = start;
            this.end = end;
        }
    }
}