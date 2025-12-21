import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        // rewrite is optional; we keep the /api prefix so backend routes match
        // rewrite: (path) => path.replace(/^\/api/, '/api')
      }
    }
  }
});