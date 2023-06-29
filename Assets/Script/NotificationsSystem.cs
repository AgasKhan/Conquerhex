using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

public class NotificationsSystem : MonoBehaviour
{
    /*
    #if UNITY_ANDROID
    DateTime savedTime;
    //DateTime timePassed;

    void Start()
    {
        Cancelar Notificaciones
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllNotifications();
        


        var chanel = new AndroidNotificationChannel("reminder_notif_chanel", "Reminder Notification", "Disturb the user", Importance.High);

        AndroidNotificationCenter.RegisterNotificationChannel(chanel);

        var notification = new AndroidNotification();

        notification.Title = "Mas te vale seguir jugando";
        notification.Text = "Segui jugando, que te cuesta escuincle";
        notification.SmallIcon = "icon_0"; // Project Settings / Mobile Notifications
        notification.LargeIcon = "icon_1";
        notification.FireTime = DateTime.Now.AddMinutes(2);
        notification.RepeatInterval = TimeSpan.FromMinutes(2);

        AndroidNotificationCenter.SendNotification(notification, chanel.Id);

    }

    public void SaveTime()
    {
        savedTime = DateTime.Now;
        SaveWithJSON.SaveInPictionary("SavedTime", savedTime.ToString());
    }

    public void TimePassed()
    {
        if (!SaveWithJSON.BD.ContainsKey("SavedTime"))
            return;

        string timeAsString = SaveWithJSON.LoadFromPictionary<string>("SavedTime");
        savedTime = DateTime.Parse(timeAsString);

        TimeSpan timePassed = DateTime.Now - savedTime; 
    }
    #endif
    */


#if UNITY_ANDROID
    private void Awake()
    {
        //AndroidNotificationCenter.CancelAllDisplayedNotifications();
        //AndroidNotificationCenter.CancelAllScheduledNotifications();
        
        AndroidNotificationCenter.CancelAllNotifications();

        var channel = new AndroidNotificationChannel("reminder_notif_ch", "Reminder Notification", "Disturb the user", Importance.High);

        AndroidNotificationCenter.RegisterNotificationChannel(channel);

        var notification = new AndroidNotification();

        notification.Title = "Volvé a jugar mostro";
        notification.Text = "Hace mucho que no jugas, te regalo 10 gemas";
        notification.LargeIcon = "icon_0";
        notification.FireTime = DateTime.Now.AddMinutes(2);
        notification.RepeatInterval = TimeSpan.FromMinutes(2);


        AndroidNotificationCenter.SendNotification(notification, channel.Id);

    }

    public static int SendNotification(DateTime schedule, string title, string text, string channelID)
    {
        
        var notification = new AndroidNotification();

        notification.Title = title;
        notification.Text = text;
        notification.SmallIcon = "icon_reminderS";
        notification.LargeIcon = "icon_reminderL";
        notification.FireTime = schedule;


        return AndroidNotificationCenter.SendNotification(notification, channelID);

    }

    public static void CancelNotification(int id)
    {
        
        AndroidNotificationCenter.CancelNotification(id);

    }
#endif
}
