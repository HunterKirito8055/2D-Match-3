using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseDictionary : MonoBehaviour
{
    Dictionary<string, object> defaults = new Dictionary<string, object>();
    void Start()
    {
        // These are the values that are used if we haven't fetched data from the
        // server
        // yet, or if we ask for values that the server doesn't have:
        defaults.Add("config_test_string", "default local string");
        defaults.Add("config_test_int", 1);
        defaults.Add("config_test_float", 1.0);
        defaults.Add("config_test_bool", false);

        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
