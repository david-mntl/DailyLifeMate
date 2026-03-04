import { API_BASE_URL } from '@/config/constants';
import { ApiError } from '@/types';

interface RequestOptions extends RequestInit {
  params?: Record<string, string | number | boolean | undefined>;
}

export async function apiClient<TResponse>(
  endpoint: string,
  options?: RequestOptions
): Promise<TResponse> {
  const { params, headers, body, ...customConfig } = options || {};

  const config: RequestInit = {
    method: options?.method || 'GET',
    headers: {
      'Content-Type': 'application/json',
      ...headers,
    },
    ...customConfig,
  };

  if (body) {
    config.body = JSON.stringify(body);
  }

  let url = `${API_BASE_URL}${endpoint}`;
  if (params) {
    const query = new URLSearchParams(
      Object.entries(params)
        .filter(([, value]) => value !== undefined)
        .map(([key, value]) => [key, String(value)])
    ).toString();
    if (query) {
      url = `${url}?${query}`;
    }
  }

  const response = await fetch(url, config);

  if (!response.ok) {
    let errorData: ApiError = {
      message: `HTTP error! status: ${response.status}`,
      statusCode: response.status,
    };
    try {
      const json = await response.json();
      errorData = { ...errorData, ...json };
    } catch (e) {
      // If response is not JSON, use status text
      errorData.details = response.statusText;
    }
    throw errorData;
  }

  // Handle cases where the response might be empty (e.g., 204 No Content)
  const contentType = response.headers.get('content-type');
  if (contentType && contentType.includes('application/json')) {
    return response.json() as Promise<TResponse>;
  }
  return {} as TResponse; // Return empty object for non-JSON responses
}
