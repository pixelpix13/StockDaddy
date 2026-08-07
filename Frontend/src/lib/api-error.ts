/** Extract a human-readable message from Axios or generic errors for toast display. */
import axios from 'axios';

export function getApiErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    if (error.code === 'ERR_NETWORK' || !error.response) {
      return 'Cannot reach the API. Start the backend with: dotnet run (port 5215).';
    }

    const data = error.response.data;
    if (typeof data === 'string' && data.trim()) {
      return data;
    }

    if (data && typeof data === 'object') {
      if ('message' in data && typeof data.message === 'string') {
        return data.message;
      }
      if ('title' in data && typeof data.title === 'string') {
        return data.title;
      }
    }
  }

  if (error instanceof Error && error.message) {
    return error.message;
  }

  return fallback;
}
