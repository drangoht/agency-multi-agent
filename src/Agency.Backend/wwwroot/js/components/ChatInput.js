export default {
    props: {
        modelValue: {
            type: String,
            required: true
        },
        disabled: {
            type: Boolean,
            default: false
        }
    },

    emits: ['update:modelValue', 'send'],

    methods: {
        onInput(event) {
            this.$emit('update:modelValue', event.target.value);
        },

        onKeyDown(event) {
            if (event.key === 'Enter' && !event.shiftKey) {
                event.preventDefault();
                this.$emit('send');
            }
        }
    },

    template: `
        <div class="flex items-end gap-2">
            <textarea
                class="flex-1 p-3 border rounded-lg resize-none focus:outline-none focus:border-blue-500"
                :value="modelValue"
                @input="onInput"
                @keydown="onKeyDown"
                :disabled="disabled"
                rows="3"
                placeholder="Tapez votre message ici..."
            ></textarea>
            <button
                @click="$emit('send')"
                :disabled="disabled"
                class="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
                Envoyer
            </button>
        </div>
    `
};