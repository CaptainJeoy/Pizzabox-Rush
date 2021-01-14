using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SceneManagerSO", fileName = "NewSceneManagerSO")]
public class SceneManagerSO : ScriptableObject
{
    private Dictionary<int, Vector3> CachedDistances = new Dictionary<int, Vector3>();

    public void StoreDistance(int key, Vector3 distance)
    {
        CachedDistances.Add(key, distance);
    }

    public Vector3 ReturnDistance(int key)
    {
        if (CachedDistances.ContainsKey(key))
            return CachedDistances[key];

        return new Vector3();
    }

    public void RemoveDistance(int key)
    {
        if (CachedDistances.ContainsKey(key))
            CachedDistances.Remove(key);
    }

    public void ClearDictionary()
    {
        CachedDistances.Clear();
    }
}
