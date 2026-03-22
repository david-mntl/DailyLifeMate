import FlipCard from "@/components/common/FlipCard";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardTitle } from "@/components/ui/card";
import MagicCard from "@/components/ui/magic-card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Skeleton } from "@/components/ui/skeleton";
import { cn } from "@/lib/utils";
import { AnimeDto } from "@/types";
import {
  ChevronDown,
  ChevronUp,
  ExternalLink as ExternalLinkIcon,
} from "lucide-react";
import React, { useState } from "react";

interface AnimeCardProps {
  anime: AnimeDto;
  onEdit?: (animeId: string) => void;
  onDelete?: (animeId: string) => void;
}

const DEFAULT_PLACEHOLDER_IMAGE =
  "https://images.pexels.com/photos/163036/manga-anime-girls-drawing-art-163036.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=2";

const AnimeCardFront = ({ anime }: { anime: AnimeDto }) => {
  const handleImgFallback = (e: React.SyntheticEvent<HTMLImageElement>) => {
    e.currentTarget.src = DEFAULT_PLACEHOLDER_IMAGE;
  };

  const imageUrl =
    anime.imageUrl && anime.imageUrl.startsWith("http")
      ? anime.imageUrl
      : DEFAULT_PLACEHOLDER_IMAGE;

  return (
    <MagicCard className="border border-border shadow-lg">
      <div className="relative w-full h-full overflow-hidden flex flex-col">
        <img
          src={imageUrl}
          alt={anime.name}
          onError={handleImgFallback}
          className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/40 to-transparent p-6 flex items-end">
          <CardTitle className="text-xl font-bold text-white leading-tight drop-shadow-lg line-clamp-2">
            {anime.name}
          </CardTitle>
        </div>
      </div>
    </MagicCard>
  );
};

const AnimeCardBack = ({ anime }: { anime: AnimeDto }) => {
  const [showFullSynopsis, setShowFullSynopsis] = useState(false);
  const [showFullDescription, setShowFullDescription] = useState(false);

  return (
    <Card className="w-full h-full border border-border shadow-xl bg-card overflow-hidden">
      <ScrollArea className="h-full w-full">
        <CardContent className="p-5 space-y-4">
          <div className="space-y-2">
            <h3 className="font-bold text-lg text-primary leading-tight">
              {anime.name}
            </h3>
            <div className="flex flex-wrap gap-1">
              {anime.genres && anime.genres.length > 0 && (
                <>
                  {anime.genres.slice(0, 3).map((genre) => (
                    <Badge
                      key={genre}
                      variant="secondary"
                      className="text-[10px] uppercase tracking-wider"
                    >
                      {genre}
                    </Badge>
                  ))}
                  {anime.genres.length > 3 && (
                    <Badge
                      variant="secondary"
                      className="text-[10px] uppercase tracking-wider opacity-60"
                    >
                      +{anime.genres.length - 3} more
                    </Badge>
                  )}
                </>
              )}
              <Badge
                variant="outline"
                className="text-[10px] uppercase tracking-wider"
              >
                {anime.totalEpisodes
                  ? `${anime.currentAvailableEpisodes || 0}/${anime.totalEpisodes} EPS`
                  : "Episodes Unknown"}
              </Badge>
            </div>
          </div>

          {anime.synopsis && (
            <div className="space-y-1">
              <div className="flex items-center justify-between">
                <h4 className="text-xs font-semibold text-muted-foreground uppercase tracking-tight">
                  Synopsis
                </h4>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={(e) => {
                    e.stopPropagation();
                    setShowFullSynopsis(!showFullSynopsis);
                  }}
                  className="h-auto p-0 text-[10px] text-muted-foreground hover:text-primary transition-colors flex items-center gap-1"
                  aria-label={
                    showFullSynopsis
                      ? "Show less synopsis"
                      : "Show more synopsis"
                  }
                >
                  {showFullSynopsis ? (
                    <>
                      Show less <ChevronUp className="h-3 w-3" />
                    </>
                  ) : (
                    <>
                      Show more <ChevronDown className="h-3 w-3" />
                    </>
                  )}
                </Button>
              </div>
              <p
                className={cn(
                  "text-sm text-foreground/90 leading-relaxed italic",
                  !showFullSynopsis && "line-clamp-3",
                )}
              >
                {anime.synopsis}
              </p>
            </div>
          )}

          <div className="grid grid-cols-2 gap-4 pt-2 border-t border-border/50">
            <div className="space-y-1">
              <h4 className="text-[10px] font-semibold text-muted-foreground uppercase">
                Released
              </h4>
              <p className="text-xs font-medium">
                {anime.releasedOn
                  ? new Date(anime.releasedOn).toLocaleDateString()
                  : "N/A"}
              </p>
            </div>
            {anime.airingStatus && (
              <div className="space-y-1">
                <h4 className="text-[10px] font-semibold text-muted-foreground uppercase">
                  Status
                </h4>
                <p className="text-xs font-medium">{anime.airingStatus}</p>
              </div>
            )}
            {anime.nextAirDateUtc && (
              <div className="space-y-1">
                <h4 className="text-[10px] font-semibold text-muted-foreground uppercase">
                  Next Air
                </h4>
                <p className="text-xs font-medium">
                  {new Date(anime.nextAirDateUtc).toLocaleDateString()}
                </p>
              </div>
            )}
          </div>

          {anime.description && (
            <div className="space-y-1 pt-2 border-t border-border/50">
              <div className="flex items-center justify-between">
                <h4 className="text-[10px] font-semibold text-muted-foreground uppercase tracking-tight">
                  Description
                </h4>
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={(e) => {
                    e.stopPropagation();
                    setShowFullDescription(!showFullDescription);
                  }}
                  className="h-auto p-0 text-[10px] text-muted-foreground hover:text-primary transition-colors flex items-center gap-1"
                  aria-label={
                    showFullDescription
                      ? "Show less description"
                      : "Show more description"
                  }
                >
                  {showFullDescription ? (
                    <>
                      Show less <ChevronUp className="h-3 w-3" />
                    </>
                  ) : (
                    <>
                      Show more <ChevronDown className="h-3 w-3" />
                    </>
                  )}
                </Button>
              </div>
              <p
                className={cn(
                  "text-sm text-foreground/90 leading-relaxed",
                  !showFullDescription && "line-clamp-3",
                )}
              >
                {anime.description}
              </p>
            </div>
          )}

          {anime.externalLinks && anime.externalLinks.length > 0 && (
            <div className="space-y-2 pt-2 border-t border-border/50">
              <h4 className="text-[10px] font-semibold text-muted-foreground uppercase">
                Links
              </h4>
              <div className="flex flex-col gap-2">
                {anime.externalLinks.map((link) => (
                  <a
                    key={link.url}
                    href={link.url}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex items-center gap-2 text-xs text-primary hover:text-primary/80 hover:underline transition-colors"
                  >
                    <ExternalLinkIcon className="h-3 w-3" />
                    <span className="truncate">{link.name}</span>
                  </a>
                ))}
              </div>
            </div>
          )}
        </CardContent>
      </ScrollArea>
    </Card>
  );
};

export function AnimeCard({ anime }: AnimeCardProps) {
  return (
    <FlipCard
      front={<AnimeCardFront anime={anime} />}
      back={<AnimeCardBack anime={anime} />}
      width="100%"
      height={420}
    />
  );
}

export function AnimeCardSkeleton() {
  return (
    <div className="w-full h-[420px] rounded-xl overflow-hidden border border-border bg-card">
      <Skeleton className="w-full h-[70%]" />
      <div className="p-4 space-y-3">
        <Skeleton className="h-6 w-3/4" />
        <Skeleton className="h-4 w-1/2" />
        <div className="pt-4 space-y-2">
          <Skeleton className="h-3 w-full" />
          <Skeleton className="h-3 w-full" />
          <Skeleton className="h-3 w-2/3" />
        </div>
      </div>
    </div>
  );
}
