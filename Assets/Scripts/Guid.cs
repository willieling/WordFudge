namespace WordFudge
{
    public struct Guid
    {
        private static int counter = 0;

        private readonly int guid;

        // Structs can't have parameterless constructors so we're adding one to get around it
        public Guid(int _ = 0)
        {
            guid = counter++;
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
    }
}
