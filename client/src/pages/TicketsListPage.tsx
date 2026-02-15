import {
  Alert,
  Box,
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
import { useState } from "react";

type ViewState = "loading" | "empty" | "error" | "data";

function TicketsListPage(): JSX.Element {
  const [viewState, setViewState] = useState<ViewState>("data");

  return (
    <Stack spacing={2}>
      <Typography variant="h4">Tickets</Typography>

      <Paper sx={{ p: 2 }}>
        <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
          <FormControl size="small" sx={{ minWidth: 200 }}>
            <InputLabel id="state-label">Stub state</InputLabel>
            <Select
              labelId="state-label"
              value={viewState}
              label="Stub state"
              onChange={(event) => setViewState(event.target.value as ViewState)}
            >
              <MenuItem value="data">data</MenuItem>
              <MenuItem value="loading">loading</MenuItem>
              <MenuItem value="empty">empty</MenuItem>
              <MenuItem value="error">error</MenuItem>
            </Select>
          </FormControl>

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

      {viewState === "loading" && (
        <Stack direction="row" spacing={1} alignItems="center">
          <CircularProgress size={20} />
          <Typography>Loading tickets...</Typography>
        </Stack>
      )}

      {viewState === "empty" && <Alert severity="info">No tickets yet.</Alert>}
      {viewState === "error" && <Alert severity="error">Failed to load tickets (stub).</Alert>}

      {viewState === "data" && (
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
              <TableRow>
                <TableCell>1</TableCell>
                <TableCell>Cannot access LMS</TableCell>
                <TableCell>New</TableCell>
                <TableCell>High</TableCell>
                <TableCell>IT</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </TableContainer>
      )}

      <Box />
    </Stack>
  );
}

export default TicketsListPage;
