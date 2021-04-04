using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordFudge.CloudService
{
    //needed?
    public struct VersionInfo
    {
        public readonly int majorVersion;
        public readonly int minorVersion;
        public readonly int patchVersion;

        public bool IsValid { get; private set; }

        public VersionInfo(string version)
        {
            const int NUMBER_COUNT = 3;

            if (!string.IsNullOrWhiteSpace(version))
            {
                string[] splits = version.Split('.');
                if (splits.Length == NUMBER_COUNT)
                {
                    bool parsedMajor = int.TryParse(splits[0], out majorVersion);
                    bool parsedMinor = int.TryParse(splits[0], out minorVersion);
                    bool parsedPatch = int.TryParse(splits[0], out patchVersion);

                    IsValid = parsedMajor && parsedMinor && parsedPatch;
                    if (IsValid)
                    {
                        return;
                    }
                }
            }

            majorVersion = 0;
            minorVersion = 0;
            patchVersion = 0;
            IsValid = false;
        }

        public override string ToString()
        {
            return $"{majorVersion}.{minorVersion}.{patchVersion}";
        }
    }
}
