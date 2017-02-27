using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Sprite;
using System;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.IO;
using UnityEngine.SceneManagement;

public class HttpClient : MonoBehaviour
{
    private const string IP = "119.29.143.72";
    private const int PORT = 9889;

    string loginUrl = "http://119.29.143.72/test/app/user/login";

    public delegate string MyDelegate(string url, string account, string password);

    public InputField userName;
    public InputField password;
    public string nettyKey;
    public bool loginSuccess = false;

    private Socket client;
    private string msg, ip;


    // Use this for initialization
    void Start()
    {

    }

    public void start()
    {
        try
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(IP, PORT);
            print("连接服务器成功\r\n");

            Thread threadReceive = new Thread(ReceiveMsg);
            threadReceive.IsBackground = true;
            threadReceive.Start();
        }
        catch
        {
            print("连接服务器失败\r\n");
        }
    }

    public void Send(string json)
    {
        if (client == null)
        {
            start();
        }
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        client.Send(buffer);
    }

    private void ReceiveMsg(object callback)
    {
        byte[] buffer = new byte[1024 * 1024];
        int len = 0;
        while (true)
        {
            len = client.Receive(buffer);
            //区分是客户端来了，还是消息来了  
            if (buffer[0] == 1)//客户端  
            {
                ip = Encoding.UTF8.GetString(buffer, 1, len - 1);
            }
            else//文本消息  
            {
                Debug.Log("服务器来消息啦");
                msg = Encoding.UTF8.GetString(buffer, 0, len);
                Debug.Log(msg);
                ConnectResponse response = JsonUtility.FromJson<ConnectResponse>(msg);
                if (response != null && response.errcode == 0)
                {
                    Debug.Log("ganjin载入Main Scene");
                    loginSuccess = true;
                }
                else
                {
                    print("response 解析出错");
                }
            }
        }
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(msg))
        {
            Debug.Log("服务器说：" + msg);
            msg = "";
        }
        if (!string.IsNullOrEmpty(ip))
        {
            ip = "";
        }

        if (loginSuccess)
        {
            Debug.Log("载入Main Scene");
            loginSuccess = false;
            SceneManager.LoadScene("main");
        }
    }

    public void login()
    {
        string account = userName.text;
        string pwdStr = password.text;
        MyDelegate mydelegate = new MyDelegate(LoginMethod);
        IAsyncResult result = mydelegate.BeginInvoke(loginUrl, account, pwdStr, LoginCallback, "Callback Param");
    }


    //线程函数
    public string LoginMethod(string loginUrl, string account, string password)
    {
        Debug.Log("login");
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("account", account);
        parameters.Add("password", password);
        HttpWebResponse response = HttpHelper.CreatePostHttpResponse(loginUrl, parameters);
        Debug.Log("status: " + response.StatusCode);

        StreamReader reader = new StreamReader(response.GetResponseStream());
        string json = reader.ReadToEnd();
        return json;
    }

    //异步回调函数
    public void LoginCallback(IAsyncResult data)
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
            Auth auth = new Auth();
            auth.opt = "auth";
            auth.key = loginResult.data.nettykey;
            string json = JsonUtility.ToJson(auth);
            Send(json);
        }
        else
        {
            Debug.Log("msg: " + loginResult.msg);
        }
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
    }
}
