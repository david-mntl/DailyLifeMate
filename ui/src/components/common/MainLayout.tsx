import { AddContextModal } from "@/components/context/AddContextModal";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { useContexts } from "@/hooks/query/useContexts";
import { cn } from "@/lib/utils";
import { LayoutDashboard, Menu, Minus, Plus } from "lucide-react";
import { useState } from "react";
import { NavLink } from "react-router-dom";

interface MainLayoutProps {
  children: React.ReactNode;
}

export function MainLayout({ children }: MainLayoutProps) {
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const [isAddContextModalOpen, setIsAddContextModalOpen] = useState(false);

  const { contexts, isLoading, isError, error, deleteContext, isDeleting } =
    useContexts();

  const handleDeleteContext = (contextId: string) => {
    if (
      window.confirm(
        "Are you sure you want to delete this dashboard and all its anime?",
      )
    ) {
      deleteContext(contextId);
    }
  };

  return (
    <TooltipProvider>
      {/* Container locked to viewport height to prevent white gaps */}
      <div className="flex flex-col h-screen w-full overflow-hidden bg-transparent text-foreground relative z-10">
        {/* Top Header Bar - flex-shrink-0 keeps it from squishing */}
        <header className="flex-shrink-0 h-16 bg-card/50 backdrop-blur-md border-b border-border/50 flex items-center px-6 shadow-md z-50">
          <h1 className="text-2xl font-extrabold text-primary animate-fade-in">
            Anime Dashboard
          </h1>
        </header>

        {/* Main Content Area (Sidebar + Dashboard View) */}
        <div className="flex flex-1 overflow-hidden">
          {/* Sidebar - Using flex instead of fixed to avoid layout gaps */}
          <aside
            className={cn(
              "flex flex-col bg-card/30 backdrop-blur-sm border-r border-border/50 shadow-xl transition-all duration-300 ease-in-out",
              isSidebarOpen ? "w-64" : "w-16",
            )}
          >
            <div className="flex items-center justify-between h-16 px-4 border-b border-border flex-shrink-0">
              {isSidebarOpen && (
                <h2 className="text-xl font-bold text-foreground animate-fade-in truncate">
                  Contexts
                </h2>
              )}
              <div
                className={cn(
                  "flex items-center gap-2",
                  !isSidebarOpen && "mx-auto",
                )}
              >
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
                    <TooltipContent side="right">
                      <p>Add Dashboard</p>
                    </TooltipContent>
                  </Tooltip>
                )}
                <Button
                  variant="ghost"
                  size="icon"
                  onClick={() => setIsSidebarOpen(!isSidebarOpen)}
                  className="text-muted-foreground hover:text-primary transition-colors"
                  aria-label={isSidebarOpen ? "Close sidebar" : "Open sidebar"}
                >
                  <Menu className="h-6 w-6" />
                </Button>
              </div>
            </div>

            <nav className="flex-1 overflow-y-auto p-4 space-y-2">
              {isLoading ? (
                <>
                  {[...Array(5)].map((_, i) => (
                    <Skeleton
                      key={i}
                      className={cn(
                        "h-10 rounded-lg",
                        isSidebarOpen ? "w-full" : "w-8 mx-auto",
                      )}
                    />
                  ))}
                </>
              ) : isError ? (
                <div className="text-destructive text-sm text-center p-2">
                  {error?.message || "Failed to load"}
                </div>
              ) : Array.isArray(contexts) && contexts.length > 0 ? (
                contexts.map((context) => (
                  <NavLink
                    key={context.id}
                    to={`/dashboard/${context.id}`}
                    className={({ isActive }) =>
                      cn(
                        "flex items-center p-2 rounded-lg text-muted-foreground hover:bg-primary/10 hover:text-primary transition-colors duration-200 group relative",
                        isActive &&
                          "bg-primary/30 text-primary-foreground font-semibold shadow-md",
                        isSidebarOpen ? "gap-3" : "justify-center",
                      )
                    }
                  >
                    <LayoutDashboard className="h-5 w-5 flex-shrink-0" />
                    {isSidebarOpen && (
                      <span className="flex-1 truncate">{context.name}</span>
                    )}
                    {isSidebarOpen && (
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
                      >
                        <Minus className="h-4 w-4" />
                      </Button>
                    )}
                  </NavLink>
                ))
              ) : (
                isSidebarOpen && (
                  <div className="text-center text-muted-foreground p-4 text-sm">
                    No dashboards yet.
                  </div>
                )
              )}
            </nav>
          </aside>

          {/* Main Content - Flex-1 takes up all remaining space */}
          <main className="flex-1 overflow-y-auto bg-transparent relative">
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
