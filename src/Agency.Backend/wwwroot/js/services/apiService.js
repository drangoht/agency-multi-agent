const API_BASE = '/api';  // Utilisation d'un chemin relatif

export async function startConversation(initialPrompt) {
    const response = await fetch(`${API_BASE}/start`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ initialPrompt })
    });
    
    if (!response.ok) {
        throw new Error('Failed to start conversation');
    }
    return await response.json();
}

export async function getConversation() {
    const response = await fetch(`${API_BASE}/conversation`);
    if (!response.ok) {
        throw new Error('Failed to fetch conversation');
    }
    return await response.json();
}