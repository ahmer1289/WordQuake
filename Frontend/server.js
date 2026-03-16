const express = require("express");
const http = require("http");
const { Server } = require("socket.io");
const path = require("path");

const app = express();
const server = http.createServer(app);
const io = new Server(server, { cors: { origin: "*" } });

app.use(express.static(path.join(__dirname, "public")));
app.get("/", (req, res) => {
    res.sendFile(path.join(__dirname, "public", "IdConfirm.html"));
});

app.use(express.json());

app.post("/game-state", (req, res) => {
    const { state } = req.body;
    console.log("Game State Received:", state);

    io.emit("gameStateUpdate", state);
    console.log("Emitted Game State:", state);

    res.send({ message: "Game state received" });
});

let players = {};

app.get("/Lobby", (req, res) => {
    res.sendFile(path.join(__dirname, "public", "IdConfirm.html"));
});

io.on("connection", (socket) => {
    socket.on("confirmLobby", (data) => {
        console.log("Lobby confirmed:", data.lobbyId);
        socket.emit("lobbyConfirmed", { lobbyId: data.lobbyId });
    });

    socket.on("playerReady", (data) => {
        console.log(`Player Ready: ${data.playerName} selected ${data.character}`);

        players[socket.id] = {
            playerName: data.playerName,
            character: data.character,
            ready: true,
        };

        const playerDataString = JSON.stringify(players[socket.id]);


        socket.emit("playerConfirmed", playerDataString);

        io.emit("updateLobby", Object.values(players));
    });

    socket.on("disconnect", () => {
        delete players[socket.id];
        io.emit("updateLobby", Object.values(players));
    });
});

server.listen(3000, () => console.log("Server running on http://localhost:3000")); 