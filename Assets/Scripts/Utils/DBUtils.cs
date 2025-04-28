using System.IO;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class DBUtils
    {
        public static string PersistentDBPath
            => Path.Combine(Application.persistentDataPath, "game.db");

        public static string TemplateDBPath
            => Path.Combine(Application.streamingAssetsPath, "database.db");
    }
}
