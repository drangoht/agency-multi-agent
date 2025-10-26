const { createApp, ref, onMounted, nextTick } = Vue

createApp({
    setup() {
        const messages = ref([])
        const newMessage = ref('')
        const isLoading = ref(false)
        const connection = ref(null)
        const messageContainer = ref(null)

        async function scrollToBottom() {
            await nextTick()
            const container = messageContainer.value
            if (container) {
                container.scrollTop = container.scrollHeight
            }
        }

        function addMessage(message) {
            messages.value.push(message)
            scrollToBottom()
        }

        async function setupSignalR() {
            const conn = new signalR.HubConnectionBuilder()
                .withUrl('/agentsHub')
                .withAutomaticReconnect([0, 2000, 5000, 10000])
                .configureLogging(signalR.LogLevel.Debug)
                .build()

            // Configuration des handlers SignalR
            conn.on('NewMessage', (message) => {
                console.log('New message received:', message)
                addMessage(message)
            })

            conn.on('Error', (error) => {
                console.error('Server error:', error)
                isLoading.value = false
            })

            try {
                await conn.start()
                console.log('SignalR Connected')
                // Demander l'historique initial
                await conn.invoke('RequestInitialConversation')
                return conn
            } catch (err) {
                console.error('SignalR Connection Error:', err)
                throw err
            }
        }

        async function sendMessage() {
            if (!newMessage.value.trim() || isLoading.value) return

            const messageContent = newMessage.value.trim()
            isLoading.value = true

            try {
                if (!connection.value || connection.value.state !== signalR.HubConnectionState.Connected) {
                    throw new Error('SignalR not connected')
                }

                // Vider l'input avant l'envoi
                newMessage.value = ''

                // Envoyer via SignalR
                await connection.value.invoke('StartConversation', messageContent)
            } catch (error) {
                console.error('Error sending message:', error)
                newMessage.value = messageContent // Restaurer le message en cas d'erreur
            } finally {
                isLoading.value = false
            }
        }

        onMounted(async () => {
            try {
                // Initialiser SignalR
                connection.value = await setupSignalR()
            } catch (error) {
                console.error('Error during initialization:', error)
            }
        })

        return {
            messages,
            newMessage,
            isLoading,
            sendMessage,
            messageContainer
        }
    }
}).mount('#app')