//export const API_BASE_URL = "http://localhost:8080/api";
export const API_BASE_URL = "http://localhost:8080/api";
export const API_ROUTES = {
  CONTEXTS: "/context",
  CONTEXT_BY_ID: (id: string) => `/context/${id}`,
  ANIME_BY_CONTEXT: (contextId: string) => `/context/${contextId}/anime`,
  ANIME_BY_ID: (contextId: string, animeId: string) =>
    `/context/${contextId}/anime/${animeId}`,
};
