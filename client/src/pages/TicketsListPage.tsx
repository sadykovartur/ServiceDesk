import {
  Alert,
  CircularProgress,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography
} from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { Link as RouterLink } from "react-router-dom";
import { getTickets } from "../api/tickets";

function TicketsListPage(): JSX.Element {
  const { data, isLoading, isError, error } = useQuery({
    queryKey: ["tickets"],
    queryFn: getTickets
  });

  const tickets = data ?? [];

  return (
    <Stack spacing={2}>
      <Typography variant="h4">Tickets</Typography>

      <Paper sx={{ p: 2 }}>
        <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel id="filter-status-label">Status (stub)</InputLabel>
            <Select labelId="filter-status-label" label="Status (stub)" value="all">
              <MenuItem value="all">All</MenuItem>
            </Select>
          </FormControl>

          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel id="filter-priority-label">Priority (stub)</InputLabel>
            <Select labelId="filter-priority-label" label="Priority (stub)" value="all">
              <MenuItem value="all">All</MenuItem>
            </Select>
          </FormControl>
        </Stack>
      </Paper>

      {isLoading && (
        <Stack direction="row" spacing={1} alignItems="center">
          <CircularProgress size={20} />
          <Typography>Loading tickets...</Typography>
        </Stack>
      )}

      {isError && (
        <Alert severity="error">
          Failed to load tickets: {error instanceof Error ? error.message : "unknown error"}
        </Alert>
      )}

      {!isLoading && !isError && tickets.length === 0 && <Alert severity="info">No tickets yet.</Alert>}

      {!isLoading && !isError && tickets.length > 0 && (
        <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>ID</TableCell>
                <TableCell>Title</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Priority</TableCell>
                <TableCell>Category</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {tickets.map((ticket) => (
                <TableRow key={ticket.id} hover>
                  <TableCell>{ticket.id}</TableCell>
                  <TableCell>
                    <RouterLink to={`/tickets/${ticket.id}`}>{ticket.title}</RouterLink>
                  </TableCell>
                  <TableCell>{ticket.status}</TableCell>
                  <TableCell>{ticket.priority}</TableCell>
                  <TableCell>{ticket.categoryName}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
    </Stack>
  );
}

export default TicketsListPage;
