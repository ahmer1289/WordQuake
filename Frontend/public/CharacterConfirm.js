const socket = io();
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
}

function sendReady() {
    const playerNameInput = document.getElementById("playerName");
    const characterName = pokemonNames[currentIndex];

    const playerName = playerNameInput.value.trim();

    if (playerName === "") {
        alert("Please enter your name before clicking Ready!");
        return;
    }

    const playerData = {
        playerName: playerName,
        character: characterName,
        ready: true
    };

    socket.emit("playerReady", playerData);
    console.log("Sent to server:", playerData);

    playerNameInput.value = "";
}


socket.on("connect", () => {
    console.log("Connected to server");
});

socket.on("gameStateUpdate", (state) => {
    console.log("Received Game State Update:", state);
    document.getElementById("game-status").innerText = state;
});

