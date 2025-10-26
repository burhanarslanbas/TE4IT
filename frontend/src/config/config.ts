/**
 * API Configuration
 * Environment'a göre API base URL'ini belirler
 */

// API Base URL'i environment'a göre belirle
const getApiBaseUrl = (): string => {
  const env = import.meta.env.VITE_APP_ENV;
  const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
  
  // Production
  if (env === 'production') {
    return apiBaseUrl || 'https://te4it-api.azurewebsites.net';
  }
  
  // Development
  return apiBaseUrl || 'http://localhost:5001';
};

// Frontend URL'i environment'a göre belirle
const getFrontendUrl = (): string => {
  const env = import.meta.env.VITE_APP_ENV;
  const frontendUrl = import.meta.env.VITE_FRONTEND_URL;
  
  // Production
  if (env === 'production') {
    return frontendUrl || 'https://te4it-frontend.azurestaticapps.net';
  }
  
  // Development
  return frontendUrl || 'http://localhost:3000';
};

// Export edilen konfigürasyonlar
export const API_BASE_URL = getApiBaseUrl();
export const FRONTEND_URL = getFrontendUrl();
export const APP_ENV = import.meta.env.VITE_APP_ENV || 'development';

// API endpoints
export const API_ENDPOINTS = {
  AUTH: `${API_BASE_URL}/api/v1/auth`,
  PROJECTS: `${API_BASE_URL}/api/v1/projects`,
  TASKS: `${API_BASE_URL}/api/v1/tasks`,
  USERS: `${API_BASE_URL}/api/v1/users`
};

// Log configuration (development only)
if (APP_ENV === 'development') {
  console.log('🔧 API Configuration:', {
    API_BASE_URL,
    FRONTEND_URL,
    APP_ENV
  });
}

