const express = require("express");
const http = require("http");
const { Server } = require("socket.io");

const app = express();
const server = http.createServer(app);
const io = new Server(server, {
    cors: { origin: "*" } 
});

app.use(express.static("public"));


io.on("connection", (socket) => {
    console.log("A user connected!");

    socket.on("sendWord", (word) => {
        console.log("Word received:", word);
        io.emit("serverResponse", `Word received: ${word}`);
    });

    socket.on("abilityUsed", (ability) => {
        console.log("Ability used:", ability);
        io.emit("serverResponse", `Ability activated: ${ability}`);
    });

    socket.on("disconnect", () => {
        console.log("A user disconnected.");
    });
});

server.listen(3000, () => {
    console.log("Server running on http://localhost:3000");
});
