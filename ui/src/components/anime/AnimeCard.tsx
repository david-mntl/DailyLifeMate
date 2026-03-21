import { AnimeDto, ExternalLink } from '@/types';
import { Card, CardContent, CardTitle } from '@/components/ui/card'; // Removed CardFooter, CardHeader
import { Link as LinkIcon } from 'lucide-react'; // Removed Pencil, Minus, Info, X
import { cn } from '@/lib/utils';
// Removed Badge, Tooltip components as they are no longer used in this iteration

interface AnimeCardProps {
  anime: AnimeDto;
  onEdit: (animeId: string) => void; // Keeping for now, but not used in UI
  onDelete: (animeId: string) => void; // Keeping for now, but not used in UI
}

const DEFAULT_PLACEHOLDER_IMAGE =
  'https://images.pexels.com/photos/163036/manga-anime-girls-drawing-art-163036.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2';

export function AnimeCard({ anime, onEdit, onDelete }: AnimeCardProps) {
  // Removed showDetails state as details will be displayed directly

  const imageUrl = anime.imageUrl && anime.imageUrl.startsWith('http')
    ? anime.imageUrl
    : DEFAULT_PLACEHOLDER_IMAGE;

  const renderExternalLink = (link: ExternalLink) => (
    <a
      key={link.url}
      href={link.url}
      target="_blank"
      rel="noopener noreferrer"
      className="inline-flex items-center gap-1 text-xs text-primary hover:underline transition-colors"
    >
      <LinkIcon className="h-3 w-3" />
      {link.name}
    </a>
  );

  return (
    <Card
      className="relative group overflow-hidden rounded-xl shadow-lg hover:shadow-2xl transition-all duration-300 ease-in-out transform hover:-translate-y-1 bg-card border-border"
    >
      <div className="relative w-full h-48 overflow-hidden">
        <img
          src={imageUrl}
          alt={anime.name}
          className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent p-4 flex items-end">
          <CardTitle className="text-lg font-bold text-white leading-tight drop-shadow-md">
            {anime.name}
          </CardTitle>
        </div>
      </div>

      <CardContent className="p-4 pt-2 space-y-2"> {/* Added space-y-2 for consistent spacing */}
        {/* Removed genre and airing status badges as per example */}

        {anime.description && (
          <p className="text-sm text-muted-foreground line-clamp-3">
            {anime.description}
          </p>
        )}

        {/* Display details directly if they exist */}
        {(anime.synopsis || anime.releasedOn || anime.nextAirDateUtc || anime.totalEpisodes || (anime.externalLinks && anime.externalLinks.length > 0)) && (
          <div className="space-y-2 text-sm text-textSecondary mt-4 pt-2 border-t border-border animate-fade-in">
            {anime.synopsis && (
              <p>
                <strong className="text-text">Synopsis:</strong> {anime.synopsis}
              </p>
            )}
            {anime.releasedOn && (
              <p>
                <strong className="text-text">Released:</strong> {new Date(anime.releasedOn).toLocaleDateString()}
              </p>
            )}
            {anime.nextAirDateUtc && (
              <p>
                <strong className="text-text">Next Air:</strong> {new Date(anime.nextAirDateUtc).toLocaleDateString()}
              </p>
            )}
            {anime.totalEpisodes && (
              <p>
                <strong className="text-text">Episodes:</strong> {anime.currentAvailableEpisodes || 0} / {anime.totalEpisodes}
              </p>
            )}
            {anime.externalLinks && anime.externalLinks.length > 0 && (
              <div className="flex flex-wrap gap-x-4 gap-y-2 pt-2">
                <strong className="text-text w-full">Links:</strong>
                {anime.externalLinks.map(renderExternalLink)}
              </div>
            )}
          </div>
        )}
      </CardContent>

      {/* Removed CardFooter and its buttons as per example */}
    </Card>
  );
}
