import { AddAnimeModal } from "@/components/anime/AddAnimeModal";
import { AnimeCard } from "@/components/anime/AnimeCard";
import { EditAnimeModal } from "@/components/anime/EditAnimeModal";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { useAnime } from "@/hooks/query/useAnime";
import { useContexts } from "@/hooks/query/useContexts";
import { AnimeDto } from "@/types";
import { ChevronDown, ChevronUp, Plus } from "lucide-react";
import { useState } from "react";
import { useParams } from "react-router-dom";

export function DashboardView() {
  const { contextId } = useParams<{ contextId: string }>();
  const {
    contexts,
    isLoading: isLoadingContexts,
    isError: isErrorContexts,
    error: errorContexts,
  } = useContexts();
  const {
    animeItems,
    isLoading: isLoadingAnime,
    isError: isErrorAnime,
    error: errorAnime,
    deleteAnime,
  } = useAnime(contextId || "");

  const [showDescription, setShowDescription] = useState(false);
  const [isAddAnimeModalOpen, setIsAddAnimeModalOpen] = useState(false);
  const [isEditAnimeModalOpen, setIsEditAnimeModalOpen] = useState(false);
  const [animeToEdit, setAnimeToEdit] = useState<AnimeDto | null>(null);

  const currentContext = contexts?.find((c) => c.id === contextId);

  const handleAddAnime = () => {
    setIsAddAnimeModalOpen(true);
  };

  const handleEditAnime = (animeId: string) => {
    const anime = animeItems?.find((a) => a.id === animeId);
    if (anime) {
      setAnimeToEdit(anime);
      setIsEditAnimeModalOpen(true);
    }
  };

  const handleDeleteAnime = (animeId: string) => {
    if (window.confirm("Are you sure you want to delete this anime?")) {
      deleteAnime(animeId);
    }
  };

  if (isLoadingContexts) {
    return (
      <div className="p-8 pt-4">
        {" "}
        {/* Adjusted padding for top header */}
        <Skeleton className="h-10 w-1/2 mb-6" />
        <Skeleton className="h-6 w-1/3 mb-8" />
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {[...Array(8)].map((_, i) => (
            <Skeleton key={i} className="h-80 rounded-xl" />
          ))}
        </div>
      </div>
    );
  }

  if (isErrorContexts || !currentContext) {
    return (
      <div className="flex flex-col items-center justify-center h-[calc(100vh-4rem)] text-center text-destructive p-8">
        {" "}
        {/* Adjusted height for fixed header */}
        <h2 className="text-3xl font-bold mb-4">Dashboard Not Found</h2>
        <p className="text-lg text-muted-foreground">
          {errorContexts?.message ||
            "The requested dashboard could not be loaded or does not exist."}
        </p>
      </div>
    );
  }

  return (
    <div className="p-8 pt-4">
      {" "}
      {/* Adjusted padding for top header */}
      <header className="flex flex-col md:flex-row md:items-center md:justify-between mb-8 pb-4 border-b border-border">
        <div className="flex items-center gap-4 mb-4 md:mb-0">
          <h1 className="text-4xl font-extrabold text-primary-foreground animate-fade-in">
            {currentContext.name}
          </h1>
          {currentContext.description && (
            <Button
              variant="ghost"
              onClick={() => setShowDescription(!showDescription)}
              className="text-muted-foreground hover:text-primary transition-colors flex items-center gap-1"
              aria-label={
                showDescription ? "Hide description" : "Show description"
              }
            >
              Description{" "}
              {showDescription ? (
                <ChevronUp className="h-4 w-4" />
              ) : (
                <ChevronDown className="h-4 w-4" />
              )}
            </Button>
          )}
        </div>
        <Button
          onClick={handleAddAnime}
          className="bg-primary hover:bg-primary/90 text-primary-foreground gap-2 rounded-lg shadow-md transition-all duration-200 ease-in-out transform hover:scale-105"
        >
          <Plus className="h-5 w-5" />
          Add Anime
        </Button>
      </header>
      {showDescription && currentContext.description && (
        <p className="text-lg text-muted-foreground mb-8 p-4 bg-card rounded-lg shadow-inner animate-slide-down">
          {currentContext.description}
        </p>
      )}
      {isLoadingAnime ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {[...Array(8)].map((_, i) => (
            <Skeleton key={i} className="h-80 rounded-xl" />
          ))}
        </div>
      ) : isErrorAnime ? (
        <div className="flex flex-col items-center justify-center h-64 text-center text-destructive">
          <h3 className="text-2xl font-bold mb-2">Failed to Load Anime</h3>
          <p className="text-muted-foreground">
            {errorAnime?.message ||
              "An error occurred while fetching anime for this dashboard."}
          </p>
        </div>
      ) : animeItems && animeItems.length > 0 ? (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {animeItems.map((anime) => (
            <AnimeCard
              key={anime.id}
              anime={anime}
              onEdit={handleEditAnime}
              onDelete={handleDeleteAnime}
            />
          ))}
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center h-64 text-center text-muted-foreground">
          <h3 className="text-2xl font-bold mb-2">No Anime Yet!</h3>
          <p className="text-lg">
            Click "Add Anime" to start building your collection.
          </p>
          <img
            src="https://images.pexels.com/photos/1036808/pexels-photo-1036808.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2"
            alt="Empty state illustration"
            className="mt-8 rounded-lg max-w-xs h-auto object-cover opacity-70"
          />
        </div>
      )}
      {contextId && (
        <AddAnimeModal
          isOpen={isAddAnimeModalOpen}
          onClose={() => setIsAddAnimeModalOpen(false)}
          contextId={contextId}
        />
      )}
      {contextId && animeToEdit && (
        <EditAnimeModal
          isOpen={isEditAnimeModalOpen}
          onClose={() => {
            setIsEditAnimeModalOpen(false);
            setAnimeToEdit(null);
          }}
          contextId={contextId}
          animeToEdit={animeToEdit}
        />
      )}
    </div>
  );
}
