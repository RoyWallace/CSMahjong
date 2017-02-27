using UnityEngine;
using System.Collections;
using System.Net;
using System.Collections.Generic;
using Assets.Sprite;
using System.Text;
using System.IO;
using System;
using System.Threading;
using System.Runtime.Remoting.Messaging;

public class Login
{

    string loginUrl = "http://119.29.143.72/test/app/user/login";

    public string account = "111";
    public string password = "222";

    public delegate string MyDelegate(string url, string account, string password);

    public void login()
    {
        MyDelegate mydelegate = new MyDelegate(TestMethod);
        IAsyncResult result = mydelegate.BeginInvoke(loginUrl, account, password, TestCallback, "Callback Param");
    }


    //线程函数
    public string TestMethod(string loginUrl, string account, string password)
    {
        Debug.Log("login");
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("account", account);
        parameters.Add("password", password);
        HttpWebResponse response = HttpHelper.CreatePostHttpResponse(loginUrl, parameters);
        Debug.Log("status: " + response.StatusCode);

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        Thread.Sleep(3000);
        return json;
    }

    //异步回调函数
    public void TestCallback(IAsyncResult data)
    {
        //Debug.Log("callback " + data);
        //Console.WriteLine(data.AsyncState);
        MyDelegate mydelegate = (MyDelegate)((AsyncResult)data).AsyncDelegate;
        //异步执行完成
        string resultstr = mydelegate.EndInvoke(data);
        BaseEntry<LoginData> loginResult = JsonUtility.FromJson<BaseEntry<LoginData>>(resultstr) as BaseEntry<LoginData>;
        Debug.Log("code: " + loginResult.code);
        Debug.Log("msg: " + loginResult.msg);
        Debug.Log("data: " + loginResult.data);
        if (loginResult.code == 0)
        {
            if (loginResult.data == null)
            {
                Debug.Log("loginResult data is null");
                return;
            }
            Debug.Log("nettyKey: " + loginResult.data.nettykey);
        }
        else
        {
            Debug.Log("msg: " + loginResult.msg);
        }

        Debug.Log("end");
    }
}
