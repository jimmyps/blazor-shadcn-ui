/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: ['class'],
  content: [
    '../NeoUI.Web.Shared/**/*.{razor,html,cs}',
    './**/*.{razor,html,cs}'
  ],
  theme: {
    extend: {},
  },
  plugins: [],
}
