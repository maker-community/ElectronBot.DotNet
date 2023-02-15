using System;
using System.Collections.Generic;
using System.Text;
using Contracts.Services;

namespace Services;
public class TuringChatbotClient : IChatbotClient
{
    public string Name => "Turing";

    public Task<string> AskQuestionResultAsync(string message) => throw new NotImplementedException();
}
