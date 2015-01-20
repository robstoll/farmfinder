using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using CH.Tutteli.FarmFinder.Dtos;
using CH.Tutteli.FarmFinder.Website.Models;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace CH.Tutteli.FarmFinder.Website
{
    public class QueueHelper
    {
         const string QueueName = "farmfinder";

        public static QueueClient QueueClient { get; set; }

        public static void Initialise()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            string connectionString = CloudConfigurationManager.GetSetting("ServiceBus.QueueConnectionString");
            QueueClient = QueueClient.CreateFromConnectionString(connectionString, QueueName);
        }

        public static void Dispose()
        {
            if (QueueClient != null)
            {
                QueueClient.Close();
            }
        }

        public static void Send(Farm farm, EUpdateMethod updateMethod)
        {
            var dto = new UpdateIndexDto
            {
                FarmId = farm.FarmId,
                UpdateTime = farm.UpdateDateTime,
                UpdateMethod = updateMethod
            };
            QueueClient.Send(new BrokeredMessage(dto));
        }
    }
}