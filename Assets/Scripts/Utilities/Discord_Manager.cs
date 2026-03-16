using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discord_Manager : MonoBehaviour
{
    Discord.Discord discord;
    
    // Start is called before the first frame update
    void Start()
    {
        try
        {
            discord = new Discord.Discord(1346518548104740916, (ulong)Discord.CreateFlags.NoRequireDiscord);
            ChangeActivity();
        }
        catch (System.Exception e)
        {
            Debug.Log("Failed to connect to Discord:" + e.Message);
        }
    }

    void OnDisable()
    {
        if (discord != null)
        {
            discord.Dispose();
        }
    }

    public void ChangeActivity()
    {
        if (discord == null)
        {
            Debug.Log("Discord is not running. Cannot display activity.");
            return;
        }
        
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity
        {
            //State = "Playing",
            Details = "Playing WordQuake"
        };
        activityManager.UpdateActivity(activity, (res) =>
        {
            Debug.Log("Activity updated");
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (discord != null)
        {
            discord.RunCallbacks();
        }
    }
}
