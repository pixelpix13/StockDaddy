import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { registerSW } from 'virtual:pwa-register'
import { ThemeProvider } from './context/ThemeContext';
import App from './App.tsx'
import './index.css'

registerSW({
  immediate: true,
  onOfflineReady() {
    console.info('StockDaddy is ready to work offline.')
  },
})

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ThemeProvider>
      <App />
    </ThemeProvider>
  </StrictMode>,
)
