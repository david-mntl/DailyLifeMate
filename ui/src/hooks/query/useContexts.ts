import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/utils/apiClient';
import { API_ROUTES } from '@/config/constants';
import { ContextDto, CreateContextRequestDto, ApiError } from '@/types';
import { useToast } from '@/hooks/use-toast'; // Corrected import path

export const CONTEXTS_QUERY_KEY = 'contexts';

export function useContexts() {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  // Query to fetch all contexts
  const { data: contexts, isLoading, isError, error } = useQuery<ContextDto[], ApiError>({
    queryKey: [CONTEXTS_QUERY_KEY],
    queryFn: () => apiClient<ContextDto[]>(API_ROUTES.CONTEXTS),
    staleTime: 1000 * 60 * 5, // 5 minutes
    onError: (err) => {
      toast({
        title: 'Failed to load dashboards',
        description: err.message || 'An unexpected error occurred.',
        variant: 'destructive',
      });
    },
  });

  // Mutation to create a new context
  const createMutation = useMutation<ContextDto, ApiError, CreateContextRequestDto>({
    mutationFn: (newContext) =>
      apiClient<ContextDto>(API_ROUTES.CONTEXTS, {
        method: 'POST',
        body: newContext,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [CONTEXTS_QUERY_KEY] });
      toast({
        title: 'Dashboard created successfully!',
        description: 'Your new anime dashboard is ready.',
      });
    },
    onError: (err) => {
      toast({
        title: 'Failed to create dashboard',
        description: err.message || 'An unexpected error occurred.',
        variant: 'destructive',
      });
    },
  });

  // Mutation to delete a context
  const deleteMutation = useMutation<void, ApiError, string>({
    mutationFn: (contextId) =>
      apiClient<void>(API_ROUTES.CONTEXT_BY_ID(contextId), {
        method: 'DELETE',
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [CONTEXTS_QUERY_KEY] });
      toast({
        title: 'Dashboard deleted successfully!',
        description: 'The anime dashboard has been removed.',
      });
    },
    onError: (err) => {
      toast({
        title: 'Failed to delete dashboard',
        description: err.message || 'An unexpected error occurred.',
        variant: 'destructive',
      });
    },
  });

  return {
    contexts,
    isLoading,
    isError,
    error,
    createContext: createMutation.mutate,
    isCreating: createMutation.isPending,
    deleteContext: deleteMutation.mutate,
    isDeleting: deleteMutation.isPending,
  };
}
