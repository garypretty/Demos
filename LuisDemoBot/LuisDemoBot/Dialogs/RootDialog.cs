using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace LuisDemoBot.Dialogs
{
    [LuisModel("LUIS_MODEL_ID", "YOUR_SUBSCRIPTION_ID")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        [LuisIntent("BookLeave")]
        public async Task BookLeave(IDialogContext context, LuisResult result)
        {
            var message = context.Activity as IMessageActivity;
            await context.PostAsync($"When you said, \"{message.Text}\", you meant you wanted to book some annual leave.");
        }

        [LuisIntent("BookMeeting")]
        public async Task CurrentAttractionWaitTime(IDialogContext context, LuisResult result)
        {
            var attendee = result.Entities.FirstOrDefault(e => e.Type == "Attendee")?.Entity;

            var durationEntity = result.Entities.FirstOrDefault(e => e.Type == "builtin.datetimeV2.duration");
            var meetingDuration = durationEntity != null
                ? EntityRecognizer.ParseTimeSpanFromDurationEntity(durationEntity)
                : null;

            var dateEntity = result.Entities.FirstOrDefault(e => e.Type == "builtin.datetimeV2.date");
            var meetingDate = dateEntity != null
                ? EntityRecognizer.ParseDateTimeFromDurationEntity(dateEntity)
                : null;

            var message = context.Activity as IMessageActivity;
            
            await context.PostAsync($"When you said, \"{message.Text}\", you meant you wanted to book a meeting.");

            if (attendee != null)
                await context.PostAsync($"The meeting will be with {attendee}");
            if (meetingDate.HasValue)
                await context.PostAsync($"The meeting will be on {meetingDate:D}");
            if (meetingDuration.HasValue)
                await context.PostAsync($"The meeting will last {meetingDuration.Value.TotalMinutes} minutes");
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry, I am not sure what you meant.");
        }

    }
}