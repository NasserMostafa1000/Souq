﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreBusinessLayer.FactoriesDP;

namespace StoreBusinessLayer.NotificationServices
{
    public static class NotificationsCreator
    {
        public static async Task SendNotification(string subject, string Body, string UserContactInfo, string NotificationsProvider)
        {

            try
            {

                var Provider = new NotificationFactory().GetNotificationProvider(NotificationsProvider);
                await Provider.SendNotification(subject, Body, UserContactInfo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
