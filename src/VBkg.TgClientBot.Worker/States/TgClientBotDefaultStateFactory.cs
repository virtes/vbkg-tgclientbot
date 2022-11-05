using Chabot;
using Chabot.State;
using Telegram.Bot.Types;

namespace VBkg.TgClientBot.Worker.States;

public class TgClientBotDefaultStateFactory : IDefaultStateFactory<Message, User>
{
    public IState CreateDefaultState(Message message, User user)
    {
        if (message.Chat.Id == user.Id)
            return DefaultState.Instance;

        return GroupDefaultState.Instance;
    }
}