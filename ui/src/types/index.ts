export interface ExternalLink {
  name: string;
  url: string;
  priority?: number;
  createdAt?: string;
}

export interface AnimeDto {
  id: string;
  name: string;
  description: string;
  imageUrl: string;
  synopsis: string;
  releasedOn?: string;
  nextAirDateUtc?: string;
  totalEpisodes?: number;
  currentAvailableEpisodes?: number;
  airingStatus: string;
  genres: string[];
  externalLinks: ExternalLink[];
  contextName: string;
  isArchived: boolean;
}

export interface CreateAnimeRequestDto {
  name: string;
  description: string;
  externalLinks?: string[];
}

export interface UpdateAnimeRequestDto {
  name: string;
  description: string;
  externalLinks?: ExternalLink[];
}

export interface DashboardItemSummaryDto {
  id: string;
  name: string;
  description: string;
}

export interface ContextDto {
  id: string;
  name: string;
  description: string;
  isArchived: boolean;
  items: DashboardItemSummaryDto[];
}

export interface CreateContextRequestDto {
  name: string;
  description: string;
}

export interface UpdateContextRequestDto {
  name: string;
  description: string;
  isArchived: boolean;
}

export interface ApiError {
  message: string;
  statusCode: number;
  details?: string;
}
