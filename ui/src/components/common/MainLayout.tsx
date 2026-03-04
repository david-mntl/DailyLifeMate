import { useState } from 'react';
import { NavLink } from 'react-router-dom';
import { useContexts } from '@/hooks/query/useContexts';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { Menu, Plus, Minus, LayoutDashboard } from 'lucide-react'; // Removed Archive as it's not in the example
import { cn } from '@/lib/utils';
import { AddContextModal } from '@/components/context/AddContextModal';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';

interface MainLayoutProps {
  children: React.ReactNode;
}

export function MainLayout({ children }: MainLayoutProps) {
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [isAddContextModalOpen, setIsAddContextModalOpen] = useState(false);

  const { contexts, isLoading, isError, error, deleteContext, isDeleting } = useContexts();

  const handleDeleteContext = (contextId: string) => {
    if (window.confirm('Are you sure you want to delete this dashboard and all its anime?')) {
      deleteContext(contextId);
    }
  };

  return (
    <TooltipProvider>
      <div className="flex flex-col min-h-screen bg-background text-text">
        {/* Top Header Bar */}
        <header className="fixed top-0 left-0 right-0 z-50 h-16 bg-card border-b border-border flex items-center px-6 shadow-lg">
          <h1 className="text-2xl font-extrabold text-primary-foreground animate-fade-in">
            Anime Dashboard
          </h1>
        </header>

        {/* Main Content Area (Sidebar + Dashboard View) */}
        <div className="flex flex-1 pt-16"> {/* pt-16 to offset fixed header */}
          {/* Sidebar */}
          <aside
            className={cn(
              'fixed inset-y-0 left-0 z-40 flex flex-col bg-card border-r border-border shadow-xl transition-all duration-300 ease-in-out pt-16', /* pt-16 to offset fixed header */
              isSidebarOpen ? 'w-64' : 'w-16'
            )}
          >
            <div className="flex items-center justify-between h-16 px-4 border-b border-border">
              {isSidebarOpen && (
                <h2 className="text-xl font-bold text-foreground animate-fade-in">Contexts</h2>
              )}
              <div className="flex items-center gap-2">
                {isSidebarOpen && (
                  <Tooltip>
                    <TooltipTrigger asChild>
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => setIsAddContextModalOpen(true)}
                        className="text-muted-foreground hover:text-primary transition-colors"
                        aria-label="Add new dashboard"
                      >
                        <Plus className="h-5 w-5" />
                      </Button>
                    </TooltipTrigger>
                    <TooltipContent className="bg-popover text-popover-foreground border-border">
                      <p>Add Dashboard</p>
                    </TooltipContent>
                  </Tooltip>
                )}
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                  className="text-muted-foreground hover:text-primary transition-colors"
                  aria-label={isSidebarOpen ? 'Close sidebar' : 'Open sidebar'}
                >
                  <Menu className="h-6 w-6" />
                </Button>
              </div>
            </div>

            <nav className="flex-1 overflow-y-auto p-4 space-y-2">
              {isLoading ? (
                <>
                  {[...Array(5)].map((_, i) => (
                    <Skeleton key={i} className={cn('h-10 rounded-lg', isSidebarOpen ? 'w-full' : 'w-8 mx-auto')} />
                  ))}
                </>
              ) : isError ? (
                <div className="text-destructive text-sm text-center p-2">
                  Failed to load dashboards: {error?.message}
                </div>
              ) : contexts && contexts.length > 0 ? (
                contexts.map((context) => (
                  <NavLink
                    key={context.id}
                    to={`/dashboard/${context.id}`}
                    className={({ isActive }) =>
                      cn(
                        'flex items-center gap-3 p-2 rounded-lg text-muted-foreground hover:bg-primary/10 hover:text-primary transition-colors duration-200 group relative',
                        isActive && 'bg-primary/30 text-primary-foreground font-semibold shadow-md',
                        !isSidebarOpen && 'justify-center'
                      )
                    }
                  >
                    <LayoutDashboard className="h-5 w-5 flex-shrink-0" />
                    {isSidebarOpen && (
                      <span className="flex-1 truncate">{context.name}</span>
                    )}
                    {isSidebarOpen && (
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={(e) => {
                              e.preventDefault();
                              e.stopPropagation();
                              handleDeleteContext(context.id);
                            }}
                            disabled={isDeleting}
                            className="h-7 w-7 text-muted-foreground hover:text-destructive opacity-0 group-hover:opacity-100 transition-opacity absolute right-2"
                            aria-label={`Delete dashboard ${context.name}`}
                          >
                            <Minus className="h-4 w-4" />
                          </Button>
                        </TooltipTrigger>
                        <TooltipContent className="bg-popover text-popover-foreground border-border">
                          <p>Delete Dashboard</p>
                        </TooltipContent>
                      </Tooltip>
                    )}
                  </NavLink>
                ))
              ) : (
                isSidebarOpen && (
                  <div className="text-center text-muted-foreground p-4">
                    No dashboards yet. Create one!
                  </div>
                )
              )}
            </nav>

            {/* "Add Dashboard" button moved to sidebar header as per example */}
          </aside>

          {/* Main Content */}
          <main
            className={cn(
              'flex-1 transition-all duration-300 ease-in-out pt-0', // pt-0 as main content starts below fixed header
              isSidebarOpen ? 'ml-64' : 'ml-16'
            )}
          >
            {children}
          </main>
        </div>

        <AddContextModal
          isOpen={isAddContextModalOpen}
          onClose={() => setIsAddContextModalOpen(false)}
        />
      </div>
    </TooltipProvider>
  );
}
