import { Navigate, Route, Routes } from "react-router-dom";
import AppLayout from "./layout/AppLayout";
import CreateTicketPage from "./pages/CreateTicketPage";
import LoginPage from "./pages/LoginPage";
import TicketDetailsPage from "./pages/TicketDetailsPage";
import TicketsListPage from "./pages/TicketsListPage";

function App(): JSX.Element {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route element={<AppLayout />}>
        <Route path="/tickets" element={<TicketsListPage />} />
        <Route path="/tickets/new" element={<CreateTicketPage />} />
        <Route path="/tickets/:id" element={<TicketDetailsPage />} />
      </Route>
      <Route path="/" element={<Navigate to="/tickets" replace />} />
      <Route path="*" element={<Navigate to="/tickets" replace />} />
    </Routes>
  );
}

export default App;
