import {
  Alert,
  Card,
  CardContent,
  CircularProgress,
  Divider,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography
} from "@mui/material";
import { useQuery } from "@tanstack/react-query";
import { useParams } from "react-router-dom";
import { getTicketById } from "../api/tickets";

function TicketDetailsPage(): JSX.Element {
  const { id } = useParams();
  const ticketId = Number(id);

  const { data: ticket, isLoading, isError, error } = useQuery({
    queryKey: ["tickets", ticketId],
    queryFn: () => getTicketById(ticketId),
    enabled: Number.isFinite(ticketId)
  });

  if (!Number.isFinite(ticketId)) {
    return <Alert severity="error">Ticket id is invalid.</Alert>;
  }

  if (isLoading) {
    return (
      <Stack direction="row" spacing={1} alignItems="center">
        <CircularProgress size={20} />
        <Typography>Loading ticket...</Typography>
      </Stack>
    );
  }

  if (isError) {
    return (
      <Alert severity="error">
        Failed to load ticket details: {error instanceof Error ? error.message : "unknown error"}
      </Alert>
    );
  }

  if (!ticket) {
    return <Alert severity="info">Ticket not found.</Alert>;
  }

  return (
    <Stack spacing={2}>
      <Typography variant="h4">Ticket #{ticket.id}</Typography>

      <Card>
        <CardContent>
          <Typography variant="h6">{ticket.title}</Typography>
          <Typography color="text.secondary">Status: {ticket.status}</Typography>
          <Typography color="text.secondary">Priority: {ticket.priority}</Typography>
          <Typography color="text.secondary">Category: {ticket.categoryName}</Typography>
          <Typography sx={{ mt: 2 }}>
            {ticket.description}
          </Typography>
        </CardContent>
      </Card>

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Comments (stub)
          </Typography>
          <Divider sx={{ mb: 1 }} />
          <List>
            <ListItem disableGutters>
              <ListItemText primary="Student: Please help, assignment deadline today." secondary="2h ago" />
            </ListItem>
            <ListItem disableGutters>
              <ListItemText primary="Operator: Investigating authentication logs." secondary="1h ago" />
            </ListItem>
          </List>
        </CardContent>
      </Card>
    </Stack>
  );
}

export default TicketDetailsPage;
