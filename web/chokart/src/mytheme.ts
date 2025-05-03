import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';

const MyTheme = definePreset(Aura, {
  semantic: {
    primary: {
      50: '#e0e7ff',
      100: '#c7d2fe',
      200: '#a5b4fc',
      300: '#818cf8',
      400: '#6366f1',
      500: '#3b82f6',
      600: '#2563eb',
      700: '#1d4ed8',
      800: '#1e40af',
      900: '#1e3a8a',
    },
    accent: {
      500: '#ffcc00',
    },
    colorScheme: {
      light: {
        surface: {
          0: '#000000', // Fondo negro
          50: '#111111',
          100: '#1a1a1a',
          200: '#222222',
        },
        highlight: {
          background: '#ffcc00',
          color: '#000000',
        },
      },
      dark: {
        surface: {
          0: '#000000',
          50: '#111111',
          100: '#1a1a1a',
        },
        highlight: {
          background: '#ffcc00',
          color: '#000000',
        },
      },
    },
  },
});

export default MyTheme;
