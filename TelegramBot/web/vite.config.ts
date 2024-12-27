import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

console.log('Vite config loaded');

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, ''),
        configure: (proxy) => {
          proxy.on('proxyReq', (proxyReq) => {
            console.log('Proxying request:', proxyReq.method, proxyReq.path);
          });
          proxy.on('error', (err) => {
            console.log('Proxy error:', err.message);
          });
        }
      }
    }
  }
});
