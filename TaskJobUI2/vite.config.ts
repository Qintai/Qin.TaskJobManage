import { defineConfig } from 'vite';
import vue from '@vitejs/plugin-vue';
import AutoImport from 'unplugin-auto-import/vite'
import Components from 'unplugin-vue-components/vite';
import { VantResolver } from 'unplugin-vue-components/resolvers';
import viteESLint from 'vite-plugin-eslint';
// eslint-disable-next-line @typescript-eslint/no-var-requires
import { ElementPlusResolver } from 'unplugin-vue-components/resolvers'

const path = require('path');

function resolve(dir: string) {
  return path.join(__dirname, dir);
}
// https://vitejs.dev/config/
// https://github.com/vitejs/vite/issues/1930 .env

export default defineConfig({
  plugins: [
    vue(),
    viteESLint(),
    AutoImport({
      resolvers: [
        ElementPlusResolver()
      ],
    }),
    Components({
      resolvers: [
        VantResolver(),
        ElementPlusResolver()
      ]
    })
  ],
  server: {
    // 关于设置代理： https://cn.vitejs.dev/config/server-options.html#server-proxy
    proxy: {
      '/TaskJob-': {
        target: 'http://localhost:9950/',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/TaskJob-/, 'TaskJob-'),
      },
    }
  },
  resolve: {
    alias: {
      '@': resolve('./src'),
      '@common': resolve('./src/common'),
      '@components': resolve('./src/components'),
      '@store': resolve('./src/store'),
      '@views': resolve('./src/views')
    }
  }
});
