﻿namespace Baku.VMagicMirror
{
    public class VRMPreviewLanguage
    {
        public const string Japanese = nameof(Japanese);
        public const string English = nameof(English);
        
        public string Language { get; private set; } = Japanese;

        public VRMPreviewLanguage(IMessageReceiver receiver)
        {
            receiver.AssignCommandHandler(
                VmmCommands.Language,
                message => Language = message.Content
            );
        }

    }
}
