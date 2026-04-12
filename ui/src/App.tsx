import CosmicBackground from "@/components/common/CosmicBackground";
import { MainLayout } from "@/components/common/MainLayout";
import { DashboardView } from "@/components/dashboard/DashboardView";
import { Toaster } from "@/components/ui/toaster";
import { Route, Routes } from "react-router-dom";
import "./App.css";

import background from "@/resources/background-pic.jpg";

function App() {
  return (
    <>
      <CosmicBackground />
      <Routes>
        <Route
          path="/"
          element={
            <MainLayout>
              <div className="flex flex-col items-center justify-center min-h-screen bg-transparent text-foreground p-8">
                <h1 className="text-5xl font-extrabold text-primary mb-4 animate-fade-in tracking-tight">
                  Welcome to Anime Dashboard
                </h1>
                <p className="text-lg text-muted-foreground text-center max-w-xl animate-slide-up">
                  Select a dashboard from the sidebar or create a new one to
                  start tracking your anime.
                </p>
                <img
                  src={background}
                  alt="Anime illustration"
                  className="mt-10 rounded-lg shadow-2xl max-w-full h-auto object-cover animate-zoom-in"
                  style={{ maxHeight: "400px" }}
                />
              </div>
            </MainLayout>
          }
        />
        <Route
          path="/dashboard/:contextId"
          element={
            <MainLayout>
              <DashboardView />
            </MainLayout>
          }
        />
      </Routes>
      <Toaster />
    </>
  );
}

export default App;
