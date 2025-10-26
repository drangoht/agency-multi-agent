export async function setupSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/agentsHub")  // Utilisation d'un chemin relatif
        .withAutomaticReconnect()
        .build();

    try {
        await connection.start();
        console.log("SignalR Connected.");
        return connection;
    } catch (err) {
        console.error("SignalR Connection Error: ", err);
        throw err;
    }
}