using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using System;

public class NotificationsSystem : MonoBehaviour
{
    DateTime savedTime;
    //DateTime timePassed;

    void Start()
    {
        /* Cancelar Notificaciones
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
        AndroidNotificationCenter.CancelAllScheduledNotifications();
        AndroidNotificationCenter.CancelAllNotifications();
        */


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

}
