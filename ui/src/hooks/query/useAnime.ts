import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/utils/apiClient';
import { API_ROUTES } from '@/config/constants';
import { AnimeDto, CreateAnimeRequestDto, UpdateAnimeRequestDto, ApiError } from '@/types';
import { useToast } from '@/hooks/use-toast';

export const ANIME_QUERY_KEY = 'anime';

export function useAnime(contextId: string) {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  // Query to fetch all anime for a specific context
  const { data: animeItems, isLoading, isError, error } = useQuery<AnimeDto[], ApiError>({
    queryKey: [ANIME_QUERY_KEY, contextId],
    queryFn: () => apiClient<AnimeDto[]>(API_ROUTES.ANIME_BY_CONTEXT(contextId)),
    enabled: !!contextId, // Only run query if contextId is available
    staleTime: 1000 * 60 * 1, // 1 minute
    onError: (err) => {
      toast({
        title: 'Failed to load anime',
        description: err.message || 'An unexpected error occurred while fetching anime.',
        variant: 'destructive',
      });
    },
  });

  // Mutation to create a new anime item
  const createMutation = useMutation<AnimeDto, ApiError, CreateAnimeRequestDto>({
    mutationFn: (newAnime) =>
      apiClient<AnimeDto>(API_ROUTES.ANIME_BY_CONTEXT(contextId), {
        method: 'POST',
        body: newAnime,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [ANIME_QUERY_KEY, contextId] });
      toast({
        title: 'Anime added successfully!',
        description: 'A new anime item has been added to your dashboard.',
      });
    },
    onError: (err) => {
      toast({
        title: 'Failed to add anime',
        description: err.message || 'An unexpected error occurred while adding anime.',
        variant: 'destructive',
      });
    },
  });

  // Mutation to update an existing anime item
  const updateMutation = useMutation<AnimeDto, ApiError, { animeId: string; data: UpdateAnimeRequestDto }>({
    mutationFn: ({ animeId, data }) =>
      apiClient<AnimeDto>(API_ROUTES.ANIME_BY_ID(contextId, animeId), {
        method: 'PUT',
        body: data,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [ANIME_QUERY_KEY, contextId] });
      toast({
        title: 'Anime updated successfully!',
        description: 'The anime item has been updated.',
      });
    },
    onError: (err) => {
      toast({
        title: 'Failed to update anime',
        description: err.message || 'An unexpected error occurred while updating anime.',
        variant: 'destructive',
      });
    },
  });

  // Mutation to delete an anime item
  const deleteMutation = useMutation<void, ApiError, string>({
    mutationFn: (animeId) =>
      apiClient<void>(API_ROUTES.ANIME_BY_ID(contextId, animeId), {
        method: 'DELETE',
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [ANIME_QUERY_KEY, contextId] });
      toast({
        title: 'Anime deleted successfully!',
        description: 'The anime item has been removed.',
      });
    },
    onError: (err) => {
      toast({
        title: 'Failed to delete anime',
        description: err.message || 'An unexpected error occurred while deleting anime.',
        variant: 'destructive',
      });
    },
  });

  return {
    animeItems,
    isLoading,
    isError,
    error,
    createAnime: createMutation.mutate,
    isCreating: createMutation.isPending,
    updateAnime: updateMutation.mutate,
    isUpdating: updateMutation.isPending,
    deleteAnime: deleteMutation.mutate,
    isDeleting: deleteMutation.isPending,
  };
}
