/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
      './Pages/**/*.cshtml',
      './Views/**/*.cshtml',
  ],
  theme: {
      fontFamily: {
          inter: ['Inter', 'sans-serif']
      },
      extend: {
          colors: {
              'ok-boss-1': '#ffece8',
              'ok-boss-2': '#fabfb6',
              'ok-boss-3': '#f59186',
              'ok-boss-4': '#f06257',
              'ok-boss-5': '#eb312b',
              'ok-boss-6': '#e60000',
              'ok-boss-7': '#c00006',
              'ok-boss-8': '#99000a',
              'ok-boss-9': '#73000b',
              'ok-boss-10': '#4d000a',
          }
      },
  },
  plugins: [],
}

