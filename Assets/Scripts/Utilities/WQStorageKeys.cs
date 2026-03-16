namespace WQUtilities
{
    public static class WQStorageKeys
    {
        /// <summary>
        /// PlayerPrefs key for selected characters.
        /// <para>We'll be storing the name of the character scriptable object along with the player's index.</para>
        /// <para>We'll also be storing the player's custom name after an underscore for the Player's name.</para>
        /// <para>UseCase:</para>
        /// <para><strong> PlayerPrefs.SetString($"{WQStorageKeys.SELECTED_PLAYER_CHARACTER_BASEKEY}{playerIndex}", $"{wqCharacter.name}_{playerName}"); </strong></para>
        /// <para>- "SELECTED_PLAYER_CHARACTER1" = "Black Cat_Promise" =  Player 1</para>
        /// <para>- "SELECTED_PLAYER_CHARACTER2" = "Tanuki_Adithya" = Player 2</para>
        /// </summary>
        public const string SELECTED_PLAYER_CHARACTER_BASEKEY = "SELECTED_PLAYER_CHARACTER";

        //
    }
}

