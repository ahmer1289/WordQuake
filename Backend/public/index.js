const socket = io('https://wordquake-090cab5a8c9f.herokuapp.com/');

let currentIndex = 0;

const pokemonNames = [
    "Black Shiba",
    "Fox",
    "Kitsune",
    "Orange Cat",
    "Pikachu",
    "Shadow Tanuki",
    "Shiba",
    "Tanuki"
];

const wordInput = document.getElementById("wordInput");
const sendWord = document.getElementById("sendWord");
const ability1 = document.getElementById("ability1");
const ability2 = document.getElementById("ability2");
const feedback = document.getElementById("feedback");


if (ability1) ability1.addEventListener("click", () => sendAbility(0));
if (ability2) ability2.addEventListener("click", () => sendAbility(1));

document.addEventListener("DOMContentLoaded", () => {
    toggleMainVisibility();
    feedback.innerText = "Wait for your turn";
    disableButtonAfterClick(sendWord);
    disableButtonAfterClick(ability1);
    disableButtonAfterClick(ability2);
});

function moveSlide(direction) {
    const carousel = document.querySelector(".carousel");
    const nameBox = document.getElementById("pokemonName");
    const totalSlides = document.querySelectorAll(".slide").length;

    currentIndex += direction;

    if (currentIndex < 0) {
        currentIndex = totalSlides - 1;
    } else if (currentIndex >= totalSlides) {
        currentIndex = 0;
    }

    carousel.style.transform = `translateX(-${currentIndex * 100}%)`;

    nameBox.style.transition = "opacity 0.2s ease-out";
    nameBox.style.opacity = 0;

    setTimeout(() => {
        nameBox.textContent = pokemonNames[currentIndex];
        nameBox.style.transition = "opacity 0.3s ease-in";
        nameBox.style.opacity = 1;
    }, 200);

    // const playerData = {
    //     roomName : "TESTINGGGG", 
    //     maxPlayers : 2
    // };

    // socket.emit("createRoom", playerData);
}

function Join() {
    const playerNameInput = document.getElementById("playerName");
    const RoomIDInput = document.getElementById("roomID");
    const gameStatus = document.getElementById("game-status");
    const characterName = pokemonNames[currentIndex];

    const playerName = playerNameInput.value.trim();
    const roomID = RoomIDInput.value.trim();

    if (playerName === "") {
        alert("Please enter your name before clicking Join!");
        return;
    }

    const playerData = {
        roomID: roomID,
        playerName: playerName,
        character: characterName,
    };

    socket.emit("JoinRoom", playerData);
}

if (wordInput && sendWord) {
    sendWord.addEventListener("click", () => {
        const word = wordInput.value.trim();
        const roomId = sessionStorage.getItem('roomId');
        const playerId = sessionStorage.getItem('playerId');
        if (word) {
            socket.emit("sendWord", word, roomId, playerId);
            wordInput.value = "";
        } else {
            alert("Word input is empty!");
        }
    });
}

function disableButtonAfterClick(button) {
    button.disabled = true;
}

function EnableButtonAfterClick(button) {
    button.disabled = false;
}

function sendAbility(abilityNumber) {
    const roomId = sessionStorage.getItem('roomId');
    const playerId = sessionStorage.getItem('playerId');
    socket.emit("abilityUsed", abilityNumber, roomId, playerId);
}

function toggleWaitVisibility() {

    const Maincontainer = document.getElementById('mainContainer');
    Maincontainer.style.display = 'none';

    const Waitcontainer = document.getElementById('waitingContainer');
    Waitcontainer.style.display = 'block';

    const container = document.getElementById('lobbyContainer');
    container.style.display = 'none';
}

function toggleMainVisibility() {

    const Maincontainer = document.getElementById('mainContainer');
    Maincontainer.style.display = 'block';

    const Waitcontainer = document.getElementById('waitingContainer');
    Waitcontainer.style.display = 'none';

    const container = document.getElementById('lobbyContainer');
    container.style.display = 'none';
}

function toggleLobbyVisibility() {

    const Maincontainer = document.getElementById('mainContainer');
    Maincontainer.style.display = 'none';

    const Waitcontainer = document.getElementById('waitingContainer');
    Waitcontainer.style.display = 'none';

    const container = document.getElementById('lobbyContainer');
    container.style.display = 'block';
}

socket.on("redirectToMain", () => {
    toggleMainVisibility();
});

socket.on("updateAbilities", ({ playerAbility1, playerAbility2 }) => {
    ability1.innerText = playerAbility1;
    ability2.innerText = playerAbility2;
});

socket.on('playerCanPlay', ({ roomId, playerId }) => {
    const RoomID = sessionStorage.getItem('roomId');
    const playerID = sessionStorage.getItem('playerId');

    if (RoomID == roomId && playerID == playerId) {
        feedback.innerText = "Your Turn";
        EnableButtonAfterClick(sendWord);
        EnableButtonAfterClick(ability1);
        EnableButtonAfterClick(ability2);

    }
});

socket.on('playerCannotPlay', ({ roomId, playerId }) => {
    const RoomID = sessionStorage.getItem('roomId');
    const playerID = sessionStorage.getItem('playerId');

    if (RoomID == roomId && playerID == playerId) {
        feedback.innerText = "Wait for your turn";
        disableButtonAfterClick(sendWord);
        disableButtonAfterClick(ability1);
        disableButtonAfterClick(ability2);
    }
});

socket.on('roomJoinSuccess', ({ message, roomId, playerId }) => {
    document.getElementById("game-status").innerText = message;
    sessionStorage.setItem('roomId', roomId);
    sessionStorage.setItem('playerId', playerId);
    toggleWaitVisibility();
});

socket.on('roomJoinError', ({ message }) => {
    document.getElementById("game-status").innerText = message;
});

socket.on('gameStarted', ({ roomId }) => {
    const RoomID = sessionStorage.getItem('roomId');
    if (RoomID == roomId) {
        toggleLobbyVisibility();
    }
});
