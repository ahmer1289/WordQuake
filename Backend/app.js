const express = require("express");
const http = require('http');
const { Server } = require("socket.io");
const path = require("path");
const redis = require('redis');
// require('dotenv').config();

const app = express();

app.use(express.static(path.join(__dirname, "public")));

const server = http.createServer(app);

const io = new Server(server, {
    cors: {
        origin: 'https://wordquake-090cab5a8c9f.herokuapp.com/',
        methods: ["GET", "POST"],
        transports: ["websocket"]
    }
});

const redisClient = redis.createClient({
    username: 'default',
    password: 'S1Wr0jShpyNPWwQPqeyFwdQpV9LoF9xL',
    socket: {
        host: 'redis-13715.c247.eu-west-1-1.ec2.redns.redis-cloud.com',
        port: 13715
    }
});

redisClient.connect().then(() => console.log("Connected to Redis!"))
    .catch(err => console.error("Redis Connection Error:", err));


const abilities = [
    "Skip Turn",
    "Extend Timer",
    "Change Theme",
    "Suggest Valid Word"
];

app.get("/", async (req, res) => {
    res.sendFile(path.join(__dirname, "public", "index.html"));
});

io.on('connection', (socket) => {

    socket.on('createRoom', async (data) => {
        const { roomName, maxPlayers } = data;

        if (![2, 3, 4].includes(maxPlayers)) {
            socket.emit('error', { code: 400, message: 'Invalid room size. Only rooms with 2, 3, or 4 players are allowed.' });
            return;
        }

        let roomId;
        let exists;

        while (exists) {
            roomId = generateUniqueRoomId();
            exists = await redisClient.exists(`room:${roomId}`);
        }

        const room = {
            id: roomId,
            name: roomName || 'Unnamed Room',
            players: [],
            maxPlayers: maxPlayers,
        };

        io.emit('roomCreated', {
            roomId: room.id,
            roomName: room.name,
            maxPlayers: room.maxPlayers,
            currentPlayers: room.players.length,
        });

        console.log(`Room created: ${room.name} (ID: ${room.id}) with max ${room.maxPlayers} players`);
        await redisClient.set(`room:${roomId}`, JSON.stringify(room));
    });

    socket.on('leaveRoom', async (data) => {
        const { roomId } = data;

        try {
            const roomData = await redisClient.get(`room:${roomId}`);

            if (!roomData) {
                socket.emit('roomLeaveError', { message: 'Room not found.' });
                return;
            }

            let room = JSON.parse(roomData);

            room.players = room.players.filter(player => player.id !== socket.id);

            io.emit('playerLeft', { playerId: socket.id, roomId });

            console.log(`Player ${socket.id} left room: ${roomId}`);

            if (room.players.length === 0) {

                await redisClient.del(`room:${roomId}`);
                io.emit('roomClosed', { roomId });
                console.log(`Room ${roomId} deleted.`);
            } else {

                await redisClient.set(`room:${roomId}`, JSON.stringify(room));
            }
            socket.leave(roomId);

        } catch (error) {
            console.error('Error leaving room:', error);
            socket.emit('roomLeaveError', { message: 'An error occurred while leaving the room.' });
        }
    });

    socket.on("JoinRoom", async (data) => {
        const { roomID, playerName, character } = data;

        try {
            const roomData = await redisClient.get(`room:${roomID}`);

            if (!roomData) {
                socket.emit('roomJoinError', { message: 'Room ID not found.' });
                return;
            }

            let room = JSON.parse(roomData);

            if (room.players.length > room.maxPlayers) {
                socket.emit('roomJoinError', { message: 'Room is full.' });
                return;
            }

            if (room.players.some(player => player.name === playerName)) {
                socket.emit('roomJoinError', { message: 'Player already in room.' });
                return;
            }

            let newPlayer = createPlayer(socket, roomID, playerName, character);
            newPlayer.abilities = getRandomAbilities();
            room.players.push(newPlayer);

            console.log("UPDATED ROOM DATA :- ", room);
            await redisClient.set(`room:${roomID}`, JSON.stringify(room));

            socket.join(roomID);
            io.emit('playerJoined', {
                playerId: newPlayer.id,
                currentPlayers: room.players.length,
                roomId: roomID,
                name: newPlayer.name,
                charactername: newPlayer.charactername
            });

            console.log(`${newPlayer.id} joined room ${roomID}`);
            socket.emit('roomJoinSuccess', {
                message: 'Joining...',
                roomId: roomID,
                playerId: newPlayer.id,
                playerTurn: 2
            });

            socket.emit('updateAbilities', {
                playerAbility1: newPlayer.abilities[0],
                playerAbility2: newPlayer.abilities[1]
            });

        } catch (error) {
            console.error('Error joining room:', error);
            socket.emit('roomJoinError', { message: 'An error occurred while joining.' });
        }
    });

    socket.on("gameStateInfo", async (data) => {

        console.log("GAME STATE DATA :- ", data);

        let result = data.split(",");
        console.log("Received Game State Update: ", result[0], " roomID: ", result[1]);

        if (result[0] === "GameOver") {
            const roomData = await redisClient.get(`room:${result[1]}`);
            console.log(`Room ${result[1]} deleted.`);
            if (roomData) {
                redisClient.del(`room:${result[1]}`).then(response => {
                    console.log(`Room ${result[1]} data deleted:`, response);
                });

                io.emit('redirectToMain');
            }

        }

        if (result[0] === "GameStarted") {
            const roomData = await redisClient.get(`room:${result[1]}`);
            if (roomData) {
                io.emit('gameStarted', {
                    roomId: result[1]
                });
                console.log("Game Started");
            }

        }
    });

    socket.on('checkturn', async (data) => {
        let result = data.split(",");

        console.log("CHECK TURNNNNNN :- ", result[1], " RoomID:- ", result[0]);

        const roomData = await redisClient.get(`room:${result[0]}`);;
        let room = JSON.parse(roomData);

        console.log("ROOM IDSSS :- ", room.id);

        if (result[0] == room.id) {
            for (let i = 0; i < room.players.length; i++) {
                if (result[1] == room.players[i].id) {
                    console.log("turn of player : ", result[1]);
                    io.emit('playerCanPlay', {
                        roomId: result[0],
                        playerId: room.players[i].id
                    });
                    console.log("Eabled inputs.......");
                }
                else {
                    io.emit('playerCannotPlay', {
                        roomId: result[0],
                        playerId: room.players[i].id
                    });
                    console.log("Disabled inputs.......");
                }
            }
        }
    });

    socket.on("sendWord", async (word, roomId, playerId) => {
        console.log("Word received:", word);

        const roomData = await redisClient.get(`room:${roomId}`);
        if (roomData) {
            let room = JSON.parse(roomData);
            if (roomId == room.id) {
                for (let i = 0; i < room.players.length; i++) {
                    if (playerId == room.players[i].id) {
                        console.log("SENDINGGG WORDDDSS ");

                        io.emit("NotifyWord", {
                            word: word,
                            roomId: roomId,
                            playerId: playerId
                        });
                    }
                }
            }
        }
    });

    socket.on("abilityUsed", async (abilityNumber, roomID, playerId) => {
        const roomData = await redisClient.get(`room:${roomID}`);
        if (roomData) {
            let room = JSON.parse(roomData);
            if (roomID == room.id) {
                for (let i = 0; i < room.players.length; i++) {
                    if (playerId == room.players[i].id) {
                        console.log("Ability used from server:", room.players[i].abilities[abilityNumber]);

                        io.emit("abilityInfo", {
                            roomId: roomID,
                            playerID: playerId,
                            abilityUsed: room.players[i].abilities[abilityNumber]
                        });

                    }
                }
            }
        }
    });
});

function generateUniqueRoomId() {
    return Math.floor(100000 + Math.random() * 900000).toString();
}

function getRandomAbilities() {
    const shuffled = abilities.sort(() => 0.5 - Math.random());
    return shuffled.slice(0, 2);
}

function createPlayer(socket, roomId, playername, charactername) {
    return {
        id: socket.id,
        name: playername,
        roomId: roomId,
        abilities: [],
        charactername: charactername,
        canPlay: false,
    };
}

const PORT = process.env.PORT || 5000;
server.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
