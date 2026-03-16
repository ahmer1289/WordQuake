[System.Serializable]
public class WQGameStateRequest
{
    public string state;
    public string roomId;

    public WQGameStateRequest(string state, string roomId)
    {
        this.state = state;
        this.roomId = roomId;
    }
}
