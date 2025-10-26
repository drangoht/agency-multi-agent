/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./wwwroot/**/*.{html,js}",
        "./wwwroot/index.html",
        "./wwwroot/js/**/*.js"
    ],
    theme: {
        extend: {
            colors: {
                'agent': {
                    'pm': '#E3F2FD',     // Product Manager
                    'dev': '#E8F5E9',    // Developer
                    'test': '#FFF3E0',   // Tester
                    'release': '#F3E5F5', // Release Manager
                },
            },
            animation: {
                'bounce-slow': 'bounce 1.5s infinite',
            }
        },
    },
    plugins: [],
    darkMode: 'class',
}