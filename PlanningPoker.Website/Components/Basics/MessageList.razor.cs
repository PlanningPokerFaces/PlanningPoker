using Microsoft.AspNetCore.Components;

namespace PlanningPoker.Website.Components.Basics;

public partial class MessageList : ComponentBase
{
    private static readonly int maxNumberOfMessages = 5;
    [Parameter] public MessageContent? MessageContent { get; set; }

    private readonly SortedList<string, MessageContent> messages = new(comparer: new InvertedStringComparer());

    protected override void OnParametersSet()
    {
        if (MessageContent == null)
        {
            return;
        }

        messages.TryAdd(MessageContent.Timestamp, MessageContent);

        if (messages.Count > maxNumberOfMessages)
        {
            var lastEntryKey = messages.Last().Key;
            messages.Remove(lastEntryKey);
        }
    }

    private sealed class InvertedStringComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            return string.Compare(y, x, StringComparison.Ordinal);
        }
    }


    private void ClearMessages()
    {
        messages.Clear();
    }
}

public sealed record MessageContent(string? Title, string? Text, string? AdditionalInfo, string Timestamp);
