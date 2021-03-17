using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationView : MonoBehaviour
{

    public static NotificationView notifInstance;

    public GameObject notificationTextobj;
    List<GameObject> poolObjectList;

    public int initialPoolSize = 5;

    private void Awake()
    {
        if (notifInstance == null)
        {
            notifInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        poolObjectList = new List<GameObject>();
        InitialisePool(initialPoolSize);
    }
  
    public void CreateNotification(string content)
    {
        GameObject notifObject = GetNotifObjectFromPool();
        notifObject.GetComponent<NotificationText>().NotificationMessage(content);
    }

    //int i = 0;
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    CreateNotification(i++.ToString());
        //}
    }

    GameObject GetNotifObjectFromPool()
    {
        foreach (var item in poolObjectList)
        {
            if (!item.activeSelf)
            {
                item.SetActive(true);
                return item;
            }
        }
        InitialisePool(5);
        return GetNotifObjectFromPool();
    }

    void InitialisePool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject newgo = Instantiate(notificationTextobj, transform);
            newgo.GetComponent<RectTransform>();
            newgo.SetActive(false);
            poolObjectList.Add(newgo);
        }
    }


}






//You can make your code faster by using a delegate Func<NotificationManager>.This allows you to change the function that is called at run-time.In your particular case calls to get the notification manager will be 3 times faster.  
//public static NotificationManager Instance
//{
//    get
//    {
//        return singlton();
//    }
//}
//private static Func<NotificationManager> singlton = () =>
//{
//    if (instance == null)
//    {
//        instance = new NotificationManager();
//    }
//    singlton = () => {
//        return instance;
//    };
//    return instance;
//};
//private static NotificationManager instance = null;