export default {
    props: {
        message: {
            type: Object,
            required: true
        }
    },
    
    computed: {
        isUserMessage() {
            return this.message.role === 'user';
        },
        
        agentColor() {
            const colors = {
                'ProductManager': 'bg-blue-100',
                'Developer': 'bg-green-100',
                'Tester': 'bg-yellow-100',
                'ReleaseManager': 'bg-purple-100',
                'default': 'bg-gray-100'
            };
            return colors[this.message.role] || colors.default;
        }
    },

    template: `
        <div :class="['flex mb-4', isUserMessage ? 'justify-end' : 'justify-start']">
            <div :class="['rounded-lg p-3 max-w-[70%]', isUserMessage ? 'bg-blue-500 text-white' : agentColor]">
                <div class="font-semibold mb-1" v-if="!isUserMessage">
                    {{ message.role }}
                </div>
                <div class="whitespace-pre-wrap">{{ message.content }}</div>
                <div class="text-xs mt-1 opacity-70">
                    {{ new Date(message.timestamp).toLocaleTimeString() }}
                </div>
            </div>
        </div>
    `
};