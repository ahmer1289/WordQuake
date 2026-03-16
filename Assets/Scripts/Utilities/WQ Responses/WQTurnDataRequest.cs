[System.Serializable]
public class WQTurnDataRequest
{
    public string roomID;
    public string currentAllowedPlayerId;

    public WQTurnDataRequest(string roomID, string currentAllowedPlayerId)
    {
        this.roomID = roomID;
        this.currentAllowedPlayerId = currentAllowedPlayerId;
    }
}
