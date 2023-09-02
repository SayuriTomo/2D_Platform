using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImagePool : MonoBehaviour
{
    [SerializeField]
    private GameObject AfterImagePreFab;

    private Queue<GameObject> AvailableObjects = new Queue<GameObject>();

    public static PlayerAfterImagePool Instance { get;private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        Instance = this;
        GrowPool();
    }

    private void GrowPool()
    {
        for(int i = 0; i < 10; i++)
        {
            var InstanceToAdd = Instantiate(AfterImagePreFab);
            InstanceToAdd.transform.SetParent(transform);
            AddToPool(InstanceToAdd);
        }
    }

    public void AddToPool(GameObject Instance)
    {
        Instance.SetActive(false);
        AvailableObjects.Enqueue(Instance);
    }

    public GameObject GetFromPool()
    {
        if(AvailableObjects.Count == 0)
        {
            GrowPool();
        }

        var Instance = AvailableObjects.Dequeue();
        Instance.SetActive(true);
        return Instance;
    }
}
