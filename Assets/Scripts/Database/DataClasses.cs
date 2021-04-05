using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.DataBase
{
    [Serializable]
    public class DatabaseVersions
    {
        public readonly Dictionary<string, int> versions = new Dictionary<string, int>();
    }
}
