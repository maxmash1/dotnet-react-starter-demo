/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        brand: {
          header: 'var(--color-brand-header)',
          content: 'var(--color-brand-content)',
          primary: 'var(--color-brand-primary)',
          text: 'var(--color-brand-text)',
        },
      },
    },
  },
  plugins: [],
};
