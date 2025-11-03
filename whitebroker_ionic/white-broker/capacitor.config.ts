import type { CapacitorConfig } from '@capacitor/cli';

const config: CapacitorConfig = {
  appId: 'com.whitebroker.app',
  appName: 'White Broker',
  webDir: 'dist',
  server: {
    cleartext: true,
    androidScheme: 'http',
    allowNavigation: ['*']
  },
  android: {
    allowMixedContent: true
  }
};

export default config;
