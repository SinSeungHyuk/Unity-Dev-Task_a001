using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    // 인스펙터에서 등록할 Pool
    [SerializeField] private List<Pool> poolArray = new List<Pool>();
    private Dictionary<EPool, ObjectPool> poolDic = new Dictionary<EPool, ObjectPool>();

    [System.Serializable]
    public struct Pool
    {
        public EPool name;
        public int initialSize;
        public GameObject prefab;
        public Transform parent; 
    }

    private class ObjectPool
    {
        // 현재 씬에 활성화 되어있는 오브젝트들 리스트
        public List<GameObject> activeObjects = new List<GameObject>();
        // 비활성화 되어있는 대기중인 오브젝트들 큐
        public Queue<GameObject> inactiveObjects = new Queue<GameObject>();
    }

    protected override void Awake()
    {
        base.Awake();

        // Awake에서 풀 미리 생성해두기
        for (int i = 0; i < poolArray.Count; ++i)
            CreatePool(poolArray[i].prefab, poolArray[i].initialSize, poolArray[i].name, poolArray[i].parent);
    }

    private void CreatePool(GameObject prefab, int size, EPool name, Transform parent)
    {
        GameObject poolContainer = new GameObject(name.ToString());
        poolContainer.transform.SetParent(parent);

        ObjectPool objectPool = new ObjectPool();
        for (int i = 0; i < size; ++i)
        {
            GameObject obj = CreateNewObject(prefab, poolContainer.transform);
            // 처음 생성된 오브젝트들은 대기큐에 들어가서 대기
            objectPool.inactiveObjects.Enqueue(obj);
        }
        poolDic.Add(name, objectPool);
    }

    private GameObject CreateNewObject(GameObject prefab, Transform parent)
    {
        GameObject obj = Instantiate(prefab, parent);
        obj.SetActive(false);
        return obj;
    }

    public GameObject Get(EPool name, Vector3 position, Quaternion rotation)
    {
        if (poolDic.TryGetValue(name, out ObjectPool objectPool))
        {
            GameObject obj;
            // 대기중인 오브젝트가 있는 큐에 접근
            if (objectPool.inactiveObjects.Count == 0)
            {
                // 풀의 모든 오브젝트가 활성화되었을 때 풀 확장
                GameObject prefab = poolArray.Find(p => p.name == name).prefab;
                obj = CreateNewObject(prefab, this.transform);
            }
            else
                // 대기중인 큐에서 오브젝트 하나 가져오기
                obj = objectPool.inactiveObjects.Dequeue();


            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            // 활성화 한 오브젝트를 활성화 리스트에 삽입
            objectPool.activeObjects.Add(obj);
            return obj;
        }

        return null;
    }
    public GameObject Get(EPool name, Transform transform)
        => Get(name, transform.position, transform.rotation);

    public void Release(GameObject obj, EPool poolName)
    {
        if (poolDic.TryGetValue(poolName, out ObjectPool objectPool))
        {
            // 반환할 오브젝트를 활성화 리스트에서 제거
            if (objectPool.activeObjects.Remove(obj))
            {
                // 반환한 오브젝트를 대기 큐에 넣어주기
                obj.SetActive(false);
                objectPool.inactiveObjects.Enqueue(obj);
            }
        }
    }
}