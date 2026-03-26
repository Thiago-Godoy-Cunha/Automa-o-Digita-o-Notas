const { defineConfig } = require("cypress");
const dotenv = require('dotenv');

// Carrega o .env e mostra as variáveis no console
const result = dotenv.config();
console.log('dotenv result:', result);      // vê se houve erro

module.exports = defineConfig({
  allowCypressEnv: true,

  e2e: {
    chromeWebSecurity: false,
    baseUrl: process.env.TEST_URL,  // opcional, para usar como baseUrl
    setupNodeEvents(on, config) {
      // Repassa as variáveis para o ambiente do Cypress
      config.env = {
        ...config.env,
        TEST_URL: process.env.TEST_URL,
        TEST_MINIMO: process.env.TEST_MINIMO,
        TEST_MAXIMO: process.env.TEST_MAXIMO,
        TEST_CENARIOS: process.env.TEST_CENARIOS,
      };
      return config;
    },
  },
});