const socket = io();

document.getElementById("confirmlobby").addEventListener("click", () => {
    const lobbyId = document.getElementById("lobbyid").value.trim();
    
    if (lobbyId === "") {
        alert("Please enter a Lobby ID.");
        return;
    }

    socket.emit("confirmLobby", { lobbyId });

    console.log(`Lobby confirmation request sent for ID: ${lobbyId}`);
});

socket.on("lobbyConfirmed", (data) => {
    alert(`Lobby ${data.lobbyId} confirmed!`);
    console.log(`Server response: Lobby ${data.lobbyId} confirmed.`);

    window.location.href = "CharacterConfirm.html";
});
