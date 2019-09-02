using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{

    // 実体を格納するための変数
    //
    // privateなので外部からはもちろん、
    // 派生クラスからもアクセスできません。
    private static T instance;

    // プロパティ
    //
    // 実体を格納した変数instanceへのアクセス窓口です。
    public static T Instance
    {
        // getアクセサ
        //
        // instanceを返します。
        get
        {
            // 初回のアクセスではinstanceは空なので、
            // シーン上から検索し、格納して返します。
            // 以降は格納されたものをそのまま返します。
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<T>();
                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "をアタッチしているGameObjectはありません");
                }
            }
            return instance;
        }
    }

}

