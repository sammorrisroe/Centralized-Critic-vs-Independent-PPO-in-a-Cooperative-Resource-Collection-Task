using UnityEngine;

public class Resource : MonoBehaviour
{
    private ResourceManager manager;

    public void Init(ResourceManager m)
    {
        manager = m;
    }

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnResourceDestroyed(gameObject);
        }
    }
}