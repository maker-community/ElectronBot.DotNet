using BotSharp.Abstraction.Users;

namespace Verdure.Braincase.Copilot.Services.BotSharp;
public class BotUserIdentity : IUserIdentity
{
    public string Id => "3b4e681a-b6af-40d0-abff-3261c735c1ec";

    public string Email => "verdure-hiro@outlook.com";

    public string UserName => "verdure-hiro";

    public string FirstName => "verdure";

    public string LastName => "hiro";

    public string FullName => "verdure hiro";

    public string? UserLanguage => null;

    public string? Phone => null;

    public string? AffiliateId => null;
}
