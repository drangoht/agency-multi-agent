export default {
    props: {
        message: {
            type: Object,
            required: true
        }
    },
    template: `
        <div class="flex justify-end mb-4">
            <div class="bg-blue-500 text-white rounded-lg p-3 max-w-[70%]">
                <div class="whitespace-pre-wrap">{{ message.content }}</div>
                <div class="text-xs mt-1 opacity-70">
                    {{ new Date(message.timestamp).toLocaleTimeString() }}
                </div>
            </div>
        </div>
    `
};