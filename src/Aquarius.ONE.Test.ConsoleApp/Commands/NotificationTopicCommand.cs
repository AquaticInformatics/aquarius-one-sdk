﻿using CommandLine;
using ONE;
using ONE.Models.CSharp;
using System;
using System.Threading.Tasks;


namespace Aquarius.ONE.Test.ConsoleApp.Commands
{
    [Verb("notificationtopics", HelpText = "Retrieve Quantity Types.")]
    public class NotificationTopicCommand : ICommand
    {
        [Option('g', "guid", Required = false, HelpText = "Unit GUID")]
        public string Guid { get; set; }
        [Option('n', "name", Required = false, HelpText = "Unit Name")]
        public string Name { get; set; }
        [Option('a', "action", Required = false, HelpText = "Action")]
        public string Action { get; set; }

        async Task<int> ICommand.Execute(ClientSDK clientSDK)
        {
            if (string.IsNullOrEmpty(Action))
            {
                if (!string.IsNullOrEmpty(Guid))
                {
                    var topic = await clientSDK.Notification.GetNotificationTopicAsync(Guid);
                    if (topic == null)
                        return 1;
                    Console.WriteLine($"Topic: {topic.Name} {topic.Description} {topic.I18NKeyName}");

                    return 0;

                }
                else
                {
                    var topics = await clientSDK.Notification.GetNotificationTopicsAsync();
                    if (topics == null)
                        return 1;
                    foreach (var topic in topics)
                    {
                        Console.WriteLine($"Topic: {topic.Id} {topic.Name} {topic.Description} {topic.I18NKeyName}");
                    }
                    return 0;
                }
            }
            if (Action.ToUpper() == "CREATE")
            {
                var notificationTopic = new NotificationTopic
                {
                    Id = "7dedc194-5fa7-4433-b97f-dc49fc89c805",
                    Name = "Limit violation alarm",
                    Description = "",
                    EnumNotificationCategory = EnumNotificationCategory.NotificationCategoryOperations,
                    I18NKeyName = new I18NKeyTextReference { I18NKey = "NOTIFICATIONTOPICS.LIMIT_VIOLATION_ALARM" }
                };
                //ActualValue,LimitValue,Location,NotificationType,Operation,Parameter,Timestamp,Unit,UserName
                notificationTopic.NotificationTopicVariables.Add(new NotificationTopicVariable { Name = "ActualValue" });
                notificationTopic.NotificationTopicVariables.Add(new NotificationTopicVariable { Name = "LimitValue" });

                var result = await clientSDK.Notification.CreateNotificationTopicAsync(notificationTopic);

                return 0;
            }
            return 0;

        }
    }
}
