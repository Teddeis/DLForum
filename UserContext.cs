public class UserContext
{
    private readonly ISession _session;

    public UserContext(ISession session)
    {
        _session = session;
    }

    public bool IsOwner(int topicUserId) =>
        _session.GetInt32("ID") == topicUserId;

    public bool IsBanned() =>
        _session.GetString("Role") == "banned";
}