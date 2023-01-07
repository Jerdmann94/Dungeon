#if UNITY_EDITOR
using ParrelSync;
#endif
using Matchplay.Shared;
using UnityEngine;


///Helps launch ParrelSynced Projects for easy testing
public class EditorJacobBootStrapper : MonoBehaviour
{
    public JacobBootStrapper jbs;


    public void Start()
    {
#if UNITY_EDITOR

        if (ClonesManager.IsClone())
        {
            var argument = ClonesManager.GetArgument();
            if (argument == "server")
            {
                Debug.Log("starting local server");
                jbs.OnParrelSyncStarted(true,"server");
            }
                    
            else if (argument == "client")
            {
                jbs.OnParrelSyncStarted(false,"client");
            }
        }
        else
           jbs.OnParrelSyncStarted(false, "client");
#endif
    }
}