using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Dynamic;
using System.Reflection;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

public class Network : MonoBehaviour
{
    static public Network Instance;
    static public Socket socket => ServerManager.socket;
    void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
        };
        When("Instantiate", (data) => {
            var go = Resources.Load((string)data.name, typeof(GameObject)) as GameObject;
            var goi = Instantiate(go, data.pos.ToObject<Vector3>(), Quaternion.identity);
            goi.name = (string)data.senderName;
        });
    }
    static public void Emit(string name, object data = null)
    {
        socket.Emit(name, JToken.FromObject(data != null ? data : new {
            name = "unnamed"
        }));
    }
    static public void Get(string name, Action<dynamic> action)
    {
        Emit("getData", new {
            name = name
        });
        When("getData", (data) => {
            if((string) data.name == name)
            {
                action(data);
            }
        });
    }
    static public void When(string name, Action<dynamic> action) {
        dynamic dt = new {};
        RefDynamic dta = new RefDynamic(ref dt);
        
        Instance.StartCoroutine(WhenUpdate(dta, action));
        socket.On(name, (d) => {
            var data = d as dynamic;
            dt = data;
            dta.value = dt;
            dta.times++;
            // action(data);
        });
    }
    static public IEnumerator WhenUpdate(RefDynamic dt, Action<dynamic> action)
    {
        int lastTimes = 0;
        while(true)
        {
            if(lastTimes != dt.times)
            {
                lastTimes = dt.times;
                action(dt.value);
            }
            yield return null;
        }
        yield return null;
    }
    static public void Instantiate(GameObject go, Vector3 pos, Vector3 euler, string senderName)
    {
        socket.Emit("Instantiate", JToken.FromObject(new {
            name = go.name,
            pos = pos,
            euler = euler,
            senderName = senderName
        }));
    }
}
public class RefDynamic
{
    public int times = 0;
    public dynamic value;
    public RefDynamic(ref dynamic reference)
    {
        value = reference;
    }
}