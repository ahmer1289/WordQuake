
const socket = io("http://localhost:3000", {
    reconnectionAttempts: 5,
    timeout: 5000, 
});

const wordInput = document.getElementById("wordInput");
const sendWord = document.getElementById("sendWord");
const ability1 = document.getElementById("ability1");
const ability2 = document.getElementById("ability2");
const ability3 = document.getElementById("ability3");
const feedback = document.getElementById("feedback");


if (wordInput && sendWord) {
    sendWord.addEventListener("click", () => {
        const word = wordInput.value.trim();
        if (word) {
            console.log("Sending word to server:", word);
            socket.emit("sendWord", word);
            wordInput.value = ""; 
        }
    });
}

if (ability1) ability1.addEventListener("click", () => sendAbility("Ability 1"));
if (ability2) ability2.addEventListener("click", () => sendAbility("Ability 2"));
if (ability3) ability3.addEventListener("click", () => sendAbility("Ability 3"));


function sendAbility(abilityName) {
    console.log("Ability used:", abilityName);
    socket.emit("abilityUsed", abilityName);
}


socket.on("serverResponse", (message) => {
    console.log("Message from server:", message);
    if (feedback) feedback.textContent = message;
});


socket.on("connect_error", (error) => {
    console.error("Connection error:", error);
});


socket.on("disconnect", () => {
    console.warn("Disconnected from server.");
});
