using Feed.Core;

namespace Feed.API.Identity;

public class IdentityContext : IIdentityContext
{
    // TODO: grab from http context accessor when auth implemented
    public Guid IdentityId => Guid.Parse("c56a4180-65aa-42ec-a945-5fd21dec0538");
}