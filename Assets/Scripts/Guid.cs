using System.Diagnostics;

namespace WordFudge
{
    [DebuggerDisplay("{guid}")]
    public struct Guid
    {
        private static int counter = 0;

        private int guid;

        public static Guid GetGuid()
        {
            Guid guid = new Guid();
            guid.guid = counter++;
            return guid;
        }

        public static bool operator ==(Guid guid, Guid other)
        {
            return guid.guid == other.guid;
        }

        public static bool operator !=(Guid guid, Guid other)
        {
            return guid.guid != other.guid;
        }

        public override bool Equals(object obj)
        {
            switch(obj)
            {
                case Guid other:
                    return guid == other.guid;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return guid;
        }

        public override string ToString()
        {
            return guid.ToString();
        }
    }
}
