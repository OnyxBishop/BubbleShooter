using UnityEngine;

namespace RamStudio.BubbleShooter.Scripts.Services
{
    public class JsonSerializer
    {
        public string Serialize<T>(T obj, bool goodPrint = false)
            => JsonUtility.ToJson(obj, goodPrint);

        public T Deserialize<T>(string json)
            => JsonUtility.FromJson<T>(json);
    }
}